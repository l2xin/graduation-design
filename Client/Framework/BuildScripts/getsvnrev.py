#!/usr/bin/env python
# coding=gbk

import os,sys
import subprocess
import time
import shutil
import re

def GetFileLastModifier(repositoryPath):
	command = 'svn info "%s"' % (repositoryPath)
	command_result = os.popen(command).read()	
	
	
	responsible_authur = ""
	last_modify_time = ""
	submatchObjs = re.finditer('^(?P<info_key>.+): (?P<info_value>.+)', command_result, re.MULTILINE)
	for submatchObj in submatchObjs:
		info_key = submatchObj.group('info_key')	
		info_value = submatchObj.group('info_value')	
		if info_key == "Last Changed Rev":
			return info_value
		if info_key == "最后修改的版本":
			return info_value
	return "1"
	
this_script_path = os.path.split(os.path.realpath(__file__))[0]
rootdir = "%s/../" % (this_script_path)	

revisionNumber = GetFileLastModifier(rootdir)
print(revisionNumber)
exit(revisionNumber)
