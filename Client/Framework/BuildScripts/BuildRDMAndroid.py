import os,sys
import subprocess
import time
import shutil


################################################
# start build
this_script_path = os.path.split(os.path.realpath(__file__))[0]
rootdir = "%s/../" % (this_script_path)

#os.chdir(rootdir)

now = time.strftime('%Y-%m-%d-%H-%M', time.localtime(time.time()))

android_sdk = os.environ.get('SDK')
unity3dPath = os.environ.get('unity3dPath')
scriptSymbols = os.environ.get('ScriptingDefineSymbols')
#print("android_sdk is:" + android_sdk)
print("unity3dPath is:" + unity3dPath)

# Replace config files
if scriptSymbols.find("LIVE_BUILD") < 0:
	shutil.copyfile(rootdir + "DiffConfigFiles/michall-inter.xml", rootdir + "Assets/Plugins/Android/assets/TSDKConfig/michall.xml")
	shutil.copyfile(rootdir + "DiffConfigFiles/msdkconfig-inter.ini", rootdir + "Assets/Plugins/Android/assets/msdkconfig.ini")
else:
	shutil.copyfile(rootdir + "DiffConfigFiles/michall-live.xml", rootdir + "Assets/Plugins/Android/assets/TSDKConfig/michall.xml")
	shutil.copyfile(rootdir + "DiffConfigFiles/msdkconfig-live.ini", rootdir + "Assets/Plugins/Android/assets/msdkconfig.ini")

if None == android_sdk:
	buildCmd = r'%s -quit -batchmode -executeMethod ProjectTool.BuildForAndroidForETC -projectPath %s -logFile %s/../log/android.log' % (unity3dPath, rootdir, rootdir)
else:
	buildCmd = r'%s -quit -batchmode -executeMethod ProjectTool.BuildForAndroidForETC -androidsdkroot %s -projectPath %s -logFile %s/../log/android.log' % (unity3dPath, android_sdk, rootdir, rootdir)

print(buildCmd)
os.system(buildCmd)

sys.exit(0)