###############################
#   Created by Frankelstner   #
###############################
#       Edited by Heico       #
###############################

###############################
# DO NOT CHANGE ANYTHING HERE #
###############################

from struct import pack,unpack
import gzip
from cStringIO import StringIO
import sys
import os
import tempfile

# General usage of the script: Drag and drop one or several files and/or folders onto the script. It will
# unpack fbrb archives and pack FbRB folders. You can also drag non-fbrb folders onto the script and specify
# whether you want to unpack or pack all fbrb folders/files within.
# There are some options in this script: activate tempfiles if the script fails at packing/unpacking,
# specify the gzip compressionlevel when packing; specify a folder to unpack/pack files to.
# Note: The script will only pack files with known extensions, e.g. xml files in a fbrb folder will be ignored (useful!)

#packing parameters:
compressionlevel=1  #takes values between 0 and 9, 0=unzipped, 1=weak compression, 9=strong compression;
                    #the vanilla files are either 0 or 5-6. The vanilla files are probably on 5-6
                    #to fit on one disk and bf3 archives are not compressed at all.
                    #While 0 can make huge files, 1 is a good compromise. Don't go higher than 6.

##unzippedfiles=("build_overlay-00.fbrb","overlay-00.fbrb","streaming_sounds-00.fbrb","streaming_vo_de-00.fbrb",
##"streaming_vo_en-00.fbrb","streaming_vo_es-00.fbrb","streaming_vo_fr-00.fbrb","streaming_vo_it-00.fbrb"
##"streaming_vo_pl-00.fbrb","streaming_vo_ru-00.fbrb","async\\ondemand_awards-00.fbrb","async\\ondemand_sounds-00.fbrb")

packtmpfile=1   #make a temporary file on the hard drive in case of memory issues
                #my 4gb system could handle all files without, except for streaming_sounds-00.fbrb (580mb)
                #no significant change in performance when packing mp_common level-00

#unpacking parameters:
unpacktmpfile=0 #temp file for unpacking, was not necessary on a 4gb system
                #extract mp_common level-00 while suppressing output (i.e. no files written)
                #14 seconds with tempfile, 7 seconds without
                #with output the difference is around 20%


#adjust unpack folder / pack file, use the commented out line below as an example (in particular, slashes);
#this line will move files into the folder "C:\Program Files (x86)\Electronic Arts\files FbRB"
#no path given puts all extracted files in a folder at the same place as the fbrb file/folder

#unpackfolder="C:/Program Files (x86)/Electronic Arts/files"
unpackfolder=""
packfolder=""

###########################

BUFFSIZE=1000000 # buffer when writing the fbrb archive


def grabstring(offset):
    # add all chars until null terminated
    re=""
    while dump[offset]!="\x00":
        re+=dump[offset]
        offset+=1
    return re
def makeint(num): return pack(">I",num)
def readint(pos): return unpack(">I",dump[pos:pos+4])[0]

#these are in fact strings on the left, the weird part of python
dic=dict(swfmovie='SwfMovie',dx10pixelshader='Dx10PixelShader',havokphysicsdata='HavokPhysicsData',
treemeshset='TreeMeshSet',terrainheightfield='TerrainHeightfield',itexture='ITexture',animtreeinfo='AnimTreeInfo',
irradiancevolume='IrradianceVolume',visualterrain='VisualTerrain',skinnedmeshset='SkinnedMeshSet',
dx10vertexshader='Dx10VertexShader',aimanimation='AimAnimation',occludermesh='OccluderMesh',
dx9shaderdatabase='Dx9ShaderDatabase',wave='Wave',sootmesh='SootMesh',terrainmaterialmap='TerrainMaterialMap',
rigidmeshset='RigidMeshSet',compositemeshset='CompositeMeshSet',watermesh='WaterMesh',visualwater='VisualWater',
dx9vertexshader='Dx9VertexShader',dx9pixelshader='Dx9PixelShader',dx11shaderdatabase='Dx11ShaderDatabase',
dx11pixelshader='Dx11PixelShader',grannymodel='GrannyModel',ragdollresource='RagdollResource',
grannyanimation='GrannyAnimation',weathersystem='WeatherSystem',dx11vertexshader='Dx11VertexShader',terrain='Terrain',
impulseresponse='ImpulseResponse',binkmemory='BinkMemory',deltaanimation='DeltaAnimation',
dx10shaderdatabase='Dx10ShaderDatabase',meshdata='MeshData',xenonpixelshader='XenonPixelShader',
xenonvertexshader='XenonVertexShader',xenonshaderdatabase='XenonShaderDatabase',xenontexture='XenonTexture',ps3pixelshader='Ps3PixelShader',
ps3vertexshader='Ps3VertexShader',ps3shaderdatabase='Ps3ShaderDatabase',ps3texture='Ps3Texture',pathdatadefinition='PathDataDefinition',
nonres='<non-resource>',dbx='<non-resource>',dbxdeleted='*deleted*',resdeleted='*deleted*',bin='<non-resource>',
dbmanifest='<non-resource>')


def packer(sourcefolder, targetfile="", compressionlevel=compressionlevel, tmpfile=1):
    """takes absoulte folder path with folder ending on " FbRB";
    the target file path is absolute without .fbrb extension"""
    sourcefolder=lp(sourcefolder)
    if not os.path.isdir(sourcefolder) or sourcefolder[-5:]!=" FbRB": return
    
    print sourcefolder[4:] ###################
    toplevellength=len(sourcefolder)+1 #for the RELATIVE pathnames to put in the fbrb

    if not targetfile: targetfile=sourcefolder[:-5]+".fbrb"
    else: targetfile=lp(targetfile)+".fbrb"
    
    strings="" #the list of strings at the beginning of part1
    extdic=dict() #keep track of all extensions to omit string duplicates in part1
    entries="" #24 bytes each, 6 parts
    numofentries=0
    payloadoffset=0 #where the uncompressed payload starts, sum of all filelengths so far
    
    if tmpfile: s2=tempfile.TemporaryFile()
    else: s2=StringIO()
    if compressionlevel: zippy2=gzip.GzipFile(fileobj=s2,mode="wb",compresslevel=compressionlevel,filename="") #takes the payload when compression

    #go through all files inside the folder
    for dir0, dirs, files in os.walk(sourcefolder):
        dir0+="\\"
        for f in files:
            #validate file and grab its extension
            rawfilename,extension = os.path.splitext(f)
            extension=extension[1:].lower()
            try:
                ext=dic[extension]
            except:
                continue
            numofentries+=1
            
            #restore filename strings to res, dbx, bin, dbmanifest; null terminated
            if extension=="dbxdeleted": filepath=dir0.replace("\\","/")[toplevellength:]+f[:-7]+"\x00"
            elif extension not in ("dbx","bin","dbmanifest"): filepath=dir0.replace("\\","/")[toplevellength:]+rawfilename+".res\x00"
            else: filepath=dir0.replace("\\","/")[toplevellength:]+f+"\x00"
            stringoffset=makeint(len(strings)) #part1/6
            filepath=str(filepath) #make string because of unicode stuff
            strings+=filepath
            
            filelength=os.path.getsize(dir0+f)
            if filelength==0: deleteflag="\x00\x00\x00\x00"
            else: deleteflag="\x00\x01\x00\x00" #part2/6
            
            #check if the extension has been used before, if so refer to the string already in use
            try:
                extpos=extdic[ext]
            except:
                extpos=len(strings)
                extdic[ext]=extpos
                strings+=ext+"\x00"

            #make the entries, grab the payload
            entries+=stringoffset+deleteflag+makeint(payloadoffset)+2*makeint(filelength)+makeint(extpos)
            payloadoffset+=filelength
            
            f1=open(dir0+f,"rb")
            if compressionlevel: zippy2.write(f1.read())
            else: s2.write(f1.read())
            f1.close()

    if compressionlevel:
        zippedflag="\x01"
        zippy2.close()
    else: 
        zippedflag="\x00"

    #make decompressed part1, then compress it
    part1="\x00\x00\x00\x02"+makeint(len(strings))+strings+makeint(numofentries)+entries+zippedflag+makeint(payloadoffset)
    s1=StringIO()
    zippy=gzip.GzipFile(fileobj=s1,mode="wb",compresslevel=1)
    zippy.write(part1)
    zippy.close()
    output=s1.getvalue()
    s1.close()
    #make the final file
    out=open(targetfile,"wb")
    s2.seek(0)
    out.write("\x46\x62\x52\x42"+makeint(len(output))+output)
    while 1:
        buff = s2.read(BUFFSIZE)
        if buff: out.write(buff)
        else: break
    out.close(), s2.close()



def unpacker(sourcefilename,targetfolder="",tmpfile=0):
    """takes absolute file path with file ending on ".fbrb";
    the target folder path is absolute without " FbRB" extension"""
    global dump
    sourcefilename=lp(sourcefilename)
    
    #check validity
    if sourcefilename[-5:].lower()!=".fbrb": return
    f=open(sourcefilename,"rb")
    if f.read(4)!="FbRB":
        f.close()
        return
    print sourcefilename[4:] ###################
    
    if not targetfolder: targetfolder=sourcefilename[:-5]+" FbRB\\"
    else: targetfolder=lp(targetfolder)+" FbRB\\"
    
    if not os.path.isdir(targetfolder): os.makedirs(targetfolder) #for empty fbrb files basically
        
    cut=unpack(">I",f.read(4))[0] # there are two gzip archives glued together
    part1=StringIO(f.read(cut))
    if tmpfile:
        part2=tempfile.TemporaryFile()
        part2.write(f.read())
        part2.seek(0)
    else:
        part2=StringIO(f.read())
    f.close()
    zippy=gzip.GzipFile(mode='rb', fileobj=part1)
    zippy2=gzip.GzipFile(mode='rb', fileobj=part2)
    
    dump=zippy.read()
    part1.close(), zippy.close()

    if dump[-5]=="\x00": zipped=0
    else: zipped=1
    
    strlen=readint(4)
    numentries=readint(strlen+8)
    for i in range(numentries):
        filenameoffset=readint(strlen+12+i*24) 
##        undeleteflag=readint(strlen+16+i*24) this is okay due to undeleteflag <=> extension=deleted
        payloadoffset=readint(strlen+20+i*24) # payload in the second gzip archive
        payloadlen=readint(strlen+24+i*24)
##        payloadlen2=readint(strlen+28+i*24) # the same as payloadlen except for one file in Package
        extensionoffset=readint(strlen+32+i*24)
    
        # get folder name, get file name, grab payload and put it in the right place
        folder,filename = os.path.split(grabstring(filenameoffset+8))
        name,ending = os.path.splitext(filename) # original file ending: bin, res, dbmanifest, dbx
        extension=grabstring(extensionoffset+8).lower() #lowercase because .itexture looks better than .ITexture
        if extension=="*deleted*":
            if ending==".dbx": ending=".dbxdeleted"
            else: ending=".resdeleted"
        elif extension=="<non-resource>" and ending==".res": ending=".nonres"
        elif extension!="<non-resource>": ending="."+extension

        finalpath=targetfolder+folder.replace("/","\\")
        if folder!="": finalpath+="\\"

        if not os.path.isdir(finalpath): os.makedirs(finalpath)
        out=open(finalpath+name+ending,"wb")
        if zipped:
            zippy2.seek(payloadoffset)
            out.write(zippy2.read(payloadlen))
        else:
            part2.seek(payloadoffset)
            out.write(part2.read(payloadlen))
        out.close()

    zippy2.close(), part2.close()



def lp(path): #long pathnames
    if path[:4]=='\\\\?\\': return path
    elif path=="": return path
    else: return unicode('\\\\?\\' + os.path.normpath(path))

#give fbrb folder->pack
#give fbrb file->extract
#give other folder->extract/pack
#sadly os.walk is rather limited for this purpose, I cannot keep it out of "marked" fbrb folders
def main():
    inp=[lp(p) for p in sys.argv[1:]]
    mode=""
    for ff in inp:
        if os.path.isdir(ff) and ff[-5:]==" FbRB":
            packer(ff,packfolder,compressionlevel,packtmpfile)
        elif os.path.isfile(ff):
            unpacker(ff,unpackfolder,unpacktmpfile)
        else: #handle all fbrb within this folder; but first ask user input
            if not mode: mode=raw_input("(u)npack or (p)ack everything from selected folder(s)\r\n")
            if mode.lower()=="u":
                for dir0,dirs,files in os.walk(ff):
                    for f in files:
                        unpacker(dir0+"\\"+f,unpackfolder,unpacktmpfile)
            elif mode.lower()=="p":
                for dir0,dirs,files in os.walk(ff):
                    packer(dir0,packfolder,compressionlevel,packtmpfile)

try:  
    main()
except Exception, e:
    raw_input(e)
