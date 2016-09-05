/*----------------------------------------------------------------
            // Copyright (C) 20015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： PakManager
            // 文件功能描述：管理所有的pak文件，pak文件管理器
            //
            // 
            // 创建标识：Neil_20150909

//----------------------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Air2000
{
	public class TextureHolder
	{
		public   Texture2D  m_Texture;
	};

	public class ByteHolder
	{
		public byte[]  m_Bytes;
	};

	public class PakFileManager
	{
	
	
		private List<PakFile>    m_InternalPaks;
		private List<PakFile>    m_OutPaks;
		private static PakFileManager m_Instance;
	

#if UNITY_EDITOR

    static private string m_TexturePath = Application.dataPath + "/../../client/";

#endif
	
		/// <summary>
		/// 
		/// </summary>
		PakFileManager ()
		{
			m_InternalPaks = new List<PakFile> ();
			m_OutPaks = new List<PakFile> ();

			initAllPakFiles ();
		}



		/// <summary>
		/// get single object 
		/// </summary>
		/// <returns></returns>
		static   public PakFileManager getInstance ()
		{
			if (m_Instance == null) {
				m_Instance = new PakFileManager ();

				return m_Instance;
			}

			return m_Instance;
		}

    




	
		/// <summary>
		/// find the  app/data path *.pak  into  m_InternalPakas
		/// find  document path *.pan    into  m_outPaks;
		/// </summary>
		void initAllPakFiles ()
		{

			m_InternalPaks.Clear ();
			m_OutPaks.Clear ();
		
			string InterPath = null;
//		string outPath=null;

			bool isFileExists = false;
			//InterPath = GTResourceManager.GetSavePath () + "/res/";

			DirectoryInfo internalDir = null;
			FileInfo[] files = null;
			if (InterPath != null) {
				internalDir = new DirectoryInfo (InterPath);
				if (internalDir.Exists) {
					//Debug.LogError("get save data path!!!!");
					files = internalDir.GetFiles ("*.PAK", SearchOption.AllDirectories);
					if (files != null && files.Length != 0) {
						isFileExists = true;
					}
				}
			}
			if (isFileExists == false) {
				//InterPath = GTResourceManager.GetDataPath ();
				if (InterPath != null) {
					internalDir = new DirectoryInfo (InterPath);
					if (internalDir.Exists) {
						//Debug.LogError("get res data path!!!!");
						files = internalDir.GetFiles ("*.PAK", SearchOption.AllDirectories);
						if (files != null && files.Length != 0) {
							isFileExists = true;
						}
					}
				}
			}

        

//#if UNITY_IPHONE
//		outPath=Application.persistentDataPath;	
//#endif

			//if (InterPath != null)
			//{
			//
			//    DirectoryInfo internalDir = new DirectoryInfo(InterPath);

			if (isFileExists == true && files != null && files.Length != 0/*&& internalDir != null && internalDir.Exists*/) {

                

				for (int i = 0; i < files.Length; ++i) {
					PakFile ptemfile = new PakFile ();

					// Helper.Log("find pak file:" + files[i].FullName);

					if (ptemfile.loadPakFile (files [i].FullName) == true) {
						Helper.Log ("load pak file success:" + files [i].FullName);
						m_InternalPaks.Add (ptemfile);

					} else {
						Helper.LogError ("load pak file failed:" + files [i].FullName);
					}

				}
			}

			//}


			//if (outPath != null)
			//{
			//
			//    DirectoryInfo outDir = new DirectoryInfo(outPath);
			//
			//    if (outDir.Exists)
			//    {
			//        FileInfo[] files = outDir.GetFiles("*.PAK", SearchOption.AllDirectories);
			//
			//        for (int i = 0; i < files.Length; ++i)
			//        {
			//            PakFile ptemfile = new PakFile();
			//
			//            Helper.Log("find pak file:" + files[i].FullName);
			//            if (ptemfile.loadPakFile(files[i].FullName) == true)
			//            {
			//                Helper.Log("load pak file success:" + files[i].FullName);
			//                m_OutPaks.Add(ptemfile);
			//
			//            }
			//            else
			//            {
			//                Helper.LogError("load pak file failed:" + files[i].FullName);
			//            }
			//
			//        }
			//    }
			//
			//
			//}
		

		
		}



		/// <summary>
		/// load file from pak file,, 
		/// </summary>
		/// <param name="filename">
		/// the file full path
		/// </param>
		/// <returns>
		/// return null if not find file     
		/// </returns>
		public  byte[] loadFileFromPak (string filename)
		{
       
			for (int i = 0; i < m_InternalPaks.Count; ++i) {
				if (m_InternalPaks [i] != null && m_InternalPaks [i].ContainFile (filename)) {
					return m_InternalPaks [i].getFileData (filename);
				}
			}


			for (int i = 0; i < m_OutPaks.Count; ++i) {
				if (m_OutPaks [i] != null && m_OutPaks [i].ContainFile (filename)) {
					return m_OutPaks [i].getFileData (filename);
				}
			}


			return null;
		}




		/// <summary>
		/// get texture2d from pak file
		/// </summary>
		/// <param name="filename">
		/// texture full file name
		/// </param>
		/// <returns>
		/// return texture2d if finded else return null
		/// </returns>
		public Texture2D getTextureFromPak (string filename)
		{
		
			/*
		
#if UNITY_EDITOR
		Texture2D ret = loadTextureFromTexturePath(filename);
		if(ret != null)
		{
			return ret;
		}
#endif
//*/



			for (int i = 0; i < m_InternalPaks.Count; ++i) {
				if (m_InternalPaks [i] != null && m_InternalPaks [i].ContainFile (filename)) {
					return m_InternalPaks [i].getTextureFromFile (filename);
				}
			}


			for (int i = 0; i < m_OutPaks.Count; ++i) {
				if (m_OutPaks [i] != null && m_OutPaks [i].ContainFile (filename)) {
					return m_OutPaks [i].getTextureFromFile (filename);
				}
			}

			string titu = "icon/titu.png";
			for (int i = 0; i < m_InternalPaks.Count; ++i) {
				if (m_InternalPaks [i] != null && m_InternalPaks [i].ContainFile (titu)) {
					return m_InternalPaks [i].getTextureFromFile (titu);
                
				}
			}
		
			for (int i = 0; i < m_OutPaks.Count; ++i) {
				if (m_OutPaks [i] != null && m_OutPaks [i].ContainFile (titu)) {
					return m_OutPaks [i].getTextureFromFile (titu);
				}
			}
		
			return null;
		}
    
	
	
	
		/// <summary>
		/// Gets the texture from pak.  asynchronous load
		/// </summary>
		/// <returns>
		/// The texture from pak.
		/// </returns>
		/// <param name='texture'>
		/// Texture.
		/// </param>
		public IEnumerator  getTextureFromPak (string filename, TextureHolder textureholder)
		{
			/*
#if UNITY_EDITOR

        textureholder.m_Texture = loadTextureFromTexturePath(filename);
		if(textureholder.m_Texture != null)
		{
			yield break;
		}
#endif
//*/

			PakFile pFile = null;
		
			for (int i = 0; i < m_InternalPaks.Count; ++i) {
				if (m_InternalPaks [i] != null && m_InternalPaks [i].ContainFile (filename)) {
					pFile = m_InternalPaks [i];
					break;
                
				}
			}
		
			if (pFile == null) {
			
				for (int i = 0; i < m_OutPaks.Count; ++i) {
					if (m_OutPaks [i] != null && m_OutPaks [i].ContainFile (filename)) {
						pFile = m_OutPaks [i];
					}
				}
			}
		

			string titu = "icon/titu.png";
		
		
			if (pFile != null) {
		    
				//Helper.Log("begin ays load texture:"+filename);
				textureholder.m_Texture = pFile.getTextureFromFile (filename);
			} else {   
				for (int i = 0; i < m_InternalPaks.Count; ++i) {
					if (m_InternalPaks [i] != null && m_InternalPaks [i].ContainFile (titu)) {
						pFile = m_InternalPaks [i];
						break;
	                
					}
				}
				if (pFile == null) {
					for (int i = 0; i < m_OutPaks.Count; ++i) {
						if (m_OutPaks [i] != null && m_OutPaks [i].ContainFile (titu)) {
							pFile = m_OutPaks [i];
						}
					}
				}
		
				textureholder.m_Texture = pFile.getTextureFromFile (titu);
				//   Helper.LogError("pFile null " + filename);
			}

		
			yield return 0;
		
		}
	


#if UNITY_EDITOR

    private Texture2D loadTextureFromTexturePath(string texturefile)
    {
        texturefile = m_TexturePath + texturefile;

        if (File.Exists(texturefile)==false)
        {
            return null;
        }

        byte[] pdata = File.ReadAllBytes(texturefile);

        if (pdata == null)
        {
            Helper.LogError("File ReadAllBytes: " + texturefile);
            return null;
        }

        Texture2D temTexture = new Texture2D(2, 2,TextureFormat.ATC_RGBA8,false);

        if (temTexture.LoadImage(pdata) == false)
        {
            return null;
        }
        return temTexture; 
        
    }



#endif



	}


}