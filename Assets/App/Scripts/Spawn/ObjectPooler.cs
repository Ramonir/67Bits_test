using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

	public Dictionary<string, Queue<GameObject>> dic_pool = new Dictionary<string, Queue<GameObject>> ();

	public GameObject SpawnFromPool (GameObject gObj, Vector3 position, Quaternion rotation, Transform parent = null) {
		string tag_gObj = gObj.name;

		GameObject gObjToSpawn = null;

		if (!dic_pool.ContainsKey (tag_gObj)) {
			gObjToSpawn = NewGameObjectToPool (tag_gObj, gObj, position, rotation, parent);
		} else {
			gObjToSpawn = dic_pool[tag_gObj].Peek ();

			if (gObjToSpawn.activeSelf) {
				gObjToSpawn = NewGameObjectToPool (tag_gObj, gObj, position, rotation, parent);
			} else {
				gObjToSpawn = dic_pool[tag_gObj].Dequeue ();

				if (parent != null) gObjToSpawn.transform.SetParent (parent);

				gObjToSpawn.transform.position = position;
				gObjToSpawn.transform.rotation = rotation;

				gObjToSpawn.SetActive (true);
			}
		}

		dic_pool[tag_gObj].Enqueue (gObjToSpawn);

		return gObjToSpawn;
	}

	GameObject NewGameObjectToPool (string tag_gObj, GameObject gObj, Vector3 position, Quaternion rotation, Transform parent = null) {
		GameObject gObj_new = Instantiate (gObj, position, rotation, parent);

		if (!dic_pool.ContainsKey (tag_gObj)) {
			Queue<GameObject> gObjPool = new Queue<GameObject> ();

			gObjPool.Enqueue (gObj_new);

			dic_pool.Add (tag_gObj, gObjPool);
		}

		return gObj_new;
	}

	public void Recicle (GameObject obj, float time = 0) {
		StartCoroutine(Coroutine_Recicle(obj, time));
	}

	IEnumerator Coroutine_Recicle(GameObject obj, float time = 0){
		yield return new WaitForSeconds(time);
		obj.SetActive(false);
	}

}