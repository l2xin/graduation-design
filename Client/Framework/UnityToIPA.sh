#!/bin/bash
time_start=$(date +%s)
path=${PWD}
XCODE_PATH=ios_proj

UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
echo "Start Build XCode project"
$UNITY_PATH -batchmode -projectPath $path -executeMethod ProjectTool.BuildForIOS project-$XCODE_PATH -quit -logFile $path/log/ios.log
#exit


cd $XCODE_PATH
xcodebuild clean

echo path is:${PWD}
rm -rf ${PWD}/../result
mkdir ${PWD}/../result

#exit

xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -destination generic/platform=iOS -archivePath bin/hlfxq.xcarchive || exit

xcodebuild -exportArchive -archivePath bin/hlfxq.xcarchive -exportPath ${PWD}/../result/hlfxq.ipa -exportFormat ipa -exportProvisioningProfile hlfxq


echo "End Build"
