#!/bin/bash

path=`pwd`
SDK=$ANDROIDSDK_LINUX_R20"/sdk"

echo "CompileEnv:"
echo $path
echo $SDK

echo "Start Build Unity to APK"

rm -rf bin/*.apk
mkdir -p bin
mkdir -p log

/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath $path -executeMethod ProjectTool.BuildForAndroid -androidsdkroot $SDK -quit -logFile $path/log/android.log

cat $path/log/*.log

echo "End Build"

