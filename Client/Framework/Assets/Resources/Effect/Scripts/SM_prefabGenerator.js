var createThis:GameObject[];  // list of possible prefabs

private var rndNr:float; // this is for just a random number holder when we need it

var thisManyTimes:int=3;
var overThisTime:float=1.0;

var xWidth:float;  // define the square where prefabs will be generated
var yWidth:float;
var zWidth:float;

var xRotMax:float;  // define maximum rotation of each prefab
var yRotMax:float=180;
var zRotMax:float;

var allUseSameRotation:boolean=false;
private var allRotationDecided:boolean=false;

var detachToWorld:boolean=true;

private var x_cur:float;  // these are used in the random palcement process
private var y_cur:float;
private var z_cur:float;


private var xRotCur:float;  // these are used in the random protation process
private var yRotCur:float;
private var zRotCur:float;

private var timeCounter:float;  // counts the time :p
private var effectCounter:int;  // you will guess ti

private var trigger:float;  // trigger: at which interwals should we generate a particle



function Start () {
if (thisManyTimes<1) thisManyTimes=1; //hack to avoid division with zero and negative numbers
trigger=overThisTime/thisManyTimes;  //define the intervals of time of the prefab generation.

}

		//function SetLayerRecursively(GameObject obj,layer)
		//{
		//	if(obj != null)
		//	{
		//		obj.layer = layer;
		//		
		//		Transform[] trans = obj.GetComponentsInChildren<Transform>();
		//		for(var i = 0;i != ob  Transform child in obj.transform)
		//		{
		//			SetLayerRecursively(child.gameObject, layer);
		//		}
		//	}
		//}

		
		//function setLayer(var obj:)
		//{
		//	
		//}
function Update () {

timeCounter+=Time.deltaTime;

	if(timeCounter>trigger&&effectCounter<=thisManyTimes)
		{
		rndNr=Mathf.Floor(Random.value*createThis.length);  //decide which prefab to create


		x_cur=transform.position.x+(Random.value*xWidth)-(xWidth*0.5);  // decide an actual place
		y_cur=transform.position.y+(Random.value*yWidth)-(yWidth*0.5);
		z_cur=transform.position.z+(Random.value*zWidth)-(zWidth*0.5);

		if(allUseSameRotation==false||allRotationDecided==false)  // basically this plays only once if allRotationDecided=true, otherwise it plays all the time
		{
		xRotCur=transform.rotation.x+(Random.value*xRotMax*2)-(xRotMax);  // decide rotation
		yRotCur=transform.rotation.y+(Random.value*yRotMax*2)-(yRotMax);  
		zRotCur=transform.rotation.z+(Random.value*zRotMax*2)-(zRotMax);  
		allRotationDecided=true;
		}

	
		var justCreated:GameObject=Instantiate(createThis[rndNr], Vector3(x_cur, y_cur, z_cur), transform.rotation);  //create the prefab
		justCreated.transform.Rotate(xRotCur, yRotCur, zRotCur);
		justCreated.layer=14;
		for(var tran in justCreated.GetComponentsInChildren(typeof(Transform)))
		{
			tran.gameObject.layer= 14;
		}
		


		if(detachToWorld==false)  // if needed we attach the freshly generated prefab to the object that is holding this script
		{
		justCreated.transform.parent=transform;
		}
		
		timeCounter-=trigger;  //administration :p
		effectCounter+=1;

		                    for (var ttr in justCreated.GetComponentsInChildren(typeof(Transform),true))
                            {
                                //ParticleSystem[] particleSystem = t.GetComponents<ParticleSystem>();
                                //if ((particleSystem != null) && (particleSystem.Length != 0))
                                {
                                    for (var temSystem in ttr.GetComponents(typeof(MeshRenderer)))
                                    {
                                        for (var cl = 0; cl < temSystem.GetComponent.<Renderer>().materials.Length; ++cl)
                                        {
                                            if (temSystem.GetComponent.<Renderer>().materials[cl].shader.isSupported == false)
                                            {
                                                Debug.Log("load not super particl shader:" + temSystem.GetComponent.<Renderer>().materials[cl].shader.name);

                                                var temss = Shader.Find(temSystem.GetComponent.<Renderer>().materials[cl].shader.name);
                                                if (temss != null)
                                                {
                                                    temSystem.GetComponent.<Renderer>().materials[cl].shader = temss;
                                                    if (temss.isSupported == false)
                                                    {
                                                        Debug.LogError("load replace shader is not supper:" + temss.name);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                //MeshRenderer[] meshRenderArray = t.GetComponents<MeshRenderer>();
                                //if (meshRenderArray != null && meshRenderArray.Length != 0)
                                //{
                                //    foreach (var mr in t.gameObject.GetComponents<MeshRenderer>())
                                //    {
                                //        // Debug.Log("Found MeshRenderer " + mr.name);
                                //        for (int c = 0; c < mr.materials.Length; ++c)
                                //        {
                                //            Material m = mr.materials[c];
                                //            Shader temfindShader = m.shader;
                                //            // Debug.Log("found get shader:" + temfindShader.name);
                                //            if (temfindShader.isSupported == false)
                                //            {
                                //                Debug.Log("load shader is not supper:" + temfindShader.name);
                                //            }                                            
                                //            Shader temloadfindShader = Shader.Find(temfindShader.name);
                                //            if (temloadfindShader != null)
                                //            {
                                //                mr.materials[c].shader = temloadfindShader;
                                //                // Debug.Log("find Editer shader success:" + temloadfindShader.name);
								//
                                //                if (m.shader.isSupported == false)
                                //                {
                                //                    Debug.LogError("set  shader is not supper:" + temfindShader.name);
                                //                }
								//
                                //            }
                                //        }
                                //    }
                                //}
                                ////SkinnedMeshRenderer[] skinMeshRenderArray = t.GetComponents<SkinnedMeshRenderer>();
                                ////if ((skinMeshRenderArray != null) && (skinMeshRenderArray.Length != 0))
                                //{
                                //    foreach (SkinnedMeshRenderer mr in t.gameObject.GetComponents<SkinnedMeshRenderer>())
                                //    {
                                //        // Debug.Log("Found MeshRenderer " + mr.name);
                                //        for (int c = 0; c < mr.materials.Length; ++c)
                                //        {
                                //            Material m = mr.materials[c];
                                //            Shader temfindShader = m.shader;
                                //            // Debug.Log("found get shader:" + temfindShader.name);
                                //            if (temfindShader.isSupported == false)
                                //            {
                                //                Debug.Log("load shader is not supper:" + temfindShader.name);
                                //            }
								//
								//
                                //            Shader temloadfindShader = Shader.Find(temfindShader.name);
                                //            if (temloadfindShader != null)
                                //            {
                                //                mr.materials[c].shader = temloadfindShader;
                                //                // Debug.Log("find Editer shader success:" + temloadfindShader.name);
                                //                if (m.shader.isSupported == false)
                                //                {
                                //                    Debug.LogError("set  shader is not supper:" + temfindShader.name);
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
								//
                                //{
                                //    foreach (TrailRenderer tr in t.gameObject.GetComponents<TrailRenderer>())
                                //    {
                                //        // Debug.Log("Found MeshRenderer " + mr.name);
                                //        for (int c = 0; c < tr.materials.Length; ++c)
                                //        {
                                //            Material m = tr.materials[c];
                                //            Shader temfindShader = m.shader;
                                //            // Debug.Log("found get shader:" + temfindShader.name);
                                //            if (temfindShader.isSupported == false)
                                //            {
                                //                Debug.Log("load shader is not supper:" + temfindShader.name);
                                //            }
								//
								//
                                //            Shader temloadfindShader = Shader.Find(temfindShader.name);
                                //            if (temloadfindShader != null)
                                //            {
                                //                tr.materials[c].shader = temloadfindShader;
                                //                // Debug.Log("find Editer shader success:" + temloadfindShader.name);
                                //                if (m.shader.isSupported == false)
                                //                {
                                //                    Debug.LogError("set  shader is not supper:" + temfindShader.name);
                                //                }
                                //            }
                                //        }
                                //    }
                                //}


                            }



		}




}