/*----------------------------------------------------------------
            // Copyright (C) 20015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： Pakfile
            // 文件功能描述：读取pak文件，pck文件可以把多个文件打成一个文件。文件没有加秘，也没有
            //做压缩，把文件的大小和名字，在文件的头部做了相关描述
            // 
            // 创建标识：Neil_20150909

//----------------------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Air2000;

namespace Air2000
{
	public class PakFile
	{

		class  FileInfor
		{
			public  int m_id;
			public int m_StartPos;
			public int m_Size;
			public string m_Path;
		};


		Dictionary<string, FileInfor> m_Files;
		private string m_FileName;

		public  PakFile ()
		{ 
		}
	
		/// <summary>
		/// Loads the pak file.
		/// </summary>
		/// <returns>
		/// return true if success else return false;
		/// </returns>
		/// <param name='filename'>
		/// If set to <c>true</c> filename.
		/// </param>
		public bool loadPakFile (string filename)
		{
			if (m_Files == null) {
				m_Files = new Dictionary<string, FileInfor> ();
			}

			m_FileName = filename;
        

			if (File.Exists (filename) == false) {
				Helper.LogError (" file is not exist");
				return false;
			} else {
			
				Helper.Log ("file find...");
			}
		
		
		
		
			m_Files.Clear ();
			FileStream fs;
			fs = File.Open (filename, FileMode.Open, FileAccess.Read);
		
			if (fs == null) {
				Helper.LogError ("File.Open Failed ");
				return false;
			} else {
				Helper.Log ("File.Open success");
			}
		
		
			if (fs.CanRead) {
				Helper.Log ("fs can read ");
			} else {
				Helper.LogError ("fs can not read");
			}
		
	
		
		
		
			byte[] bytecount = new byte[300];
			fs.Read (bytecount, 0, 4);
		
	
		
			int fileCount1 = System.BitConverter.ToInt32 (bytecount, 0);
		
		
			Helper.Log ("read file count:" + fileCount1);
		
			for (int i=0; i<fileCount1; ++i) {
				FileInfor pTemFile = new FileInfor ();
			
				fs.Read (bytecount, 0, 4);
				pTemFile.m_id = System.BitConverter.ToInt32 (bytecount, 0);
			
				fs.Read (bytecount, 0, 4);
				pTemFile.m_StartPos = System.BitConverter.ToInt32 (bytecount, 0);
			
				fs.Read (bytecount, 0, 4);
				pTemFile.m_Size = System.BitConverter.ToInt32 (bytecount, 0);
			
			
				fs.Read (bytecount, 0, 256);
			
				pTemFile.m_Path = System.Text.Encoding.Default.GetString (bytecount);
			
				int leng = pTemFile.m_Path.Length;

				for (int j = 0; j < leng; ++j) {
					char temchar = pTemFile.m_Path [j];
					if (temchar == 0) {
						pTemFile.m_Path = pTemFile.m_Path.Substring (0, j);
						break;
					}

				}
			
				if (m_Files.ContainsKey (pTemFile.m_Path) == false) {
			
					m_Files.Add (pTemFile.m_Path, pTemFile);
				}
			
				//Helper.LogError("load file id:"+pTemFile.m_id+" start:"+pTemFile.m_StartPos+" size:"+pTemFile.m_Size+" filename:"+pTemFile.m_Path);
			
			}
	
		
			fs.Close ();
			fs.Dispose ();
			return true;
		}

	
	
	
	
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// The file.
		/// </returns>
		/// <param name='filename'>
		/// If set to <c>true</c> filename.
		/// </param>
		public bool  ContainFile (string filename)
		{
			return m_Files.ContainsKey (filename);
		}


 
   
		/// <summary>
		/// Gets the file data.
		/// </summary>
		/// <returns>
		/// return null if not contain file...
		/// </returns>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		public byte[] getFileData (string fileName)
		{
			FileInfor fileInfor = null;

			bool b = m_Files.ContainsKey (fileName);

			b = m_Files.TryGetValue (fileName, out fileInfor);
			if (b == false) {
				return null;
			}

       
			FileStream fs;
			fs = File.Open (m_FileName, FileMode.Open, FileAccess.Read);
		
			if (fs == null || fs.CanRead == false) {
				Helper.LogError ("getFileData file open error ");
				return null;
			}
		
			byte[] data = new byte[fileInfor.m_Size];
			try {
				fs.Seek (fileInfor.m_StartPos, SeekOrigin.Begin);
				fs.Read (data, 0, fileInfor.m_Size);	
				fs.Close ();
				fs.Dispose ();
			
			} catch (System.Exception e) {
				Debug.LogException (e);
		
				fs.Close ();
				fs.Dispose ();
				return null;
			
			}
	
			return data;

		}

	
	
		/// <summary>
		/// Gets the texture from file.
		/// </summary>
		/// <returns>
		/// The texture from file. return null if failed
		/// </returns>
		/// <param name='filename'>
		/// Filename.
		/// </param>
	
		public Texture2D getTextureFromFile (string filename)
		{
			byte[] pdata = getFileData (filename);
			if (pdata == null) {
				return null;
			}

			Texture2D pTexture = new Texture2D (2, 2, TextureFormat.ARGB32, false);

			if (pTexture.LoadImage (pdata)) {
				return pTexture;
			}
			return null;

		}

	
		/*
	public IEnumerator getTextureFromFile(string filename,TextureHolder holder)
	{
		Helper.Log("begin get texture from file:"+ filename);
		
		if(holder==null)
		{
			yield return 0;
		}
		
		holder.m_Texture=getTextureFromFile(filename);
	
		yield return 0;
	}
	 
	 //*/


  
	}

}
