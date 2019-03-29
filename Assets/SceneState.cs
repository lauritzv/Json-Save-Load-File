using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using Random = System.Random;

namespace JsonSceneStateSavingAndLoading
{
    public class SceneState : MonoBehaviour
    {
        private Random rand = new Random();
        [SerializeField] private int randRange = 5;
        [SerializeField] private Transform SceneObjectParent;
        private List<GameObject> _sceneObjects = new List<GameObject>();
        private SceneStatus _status = new SceneStatus();
        [SerializeField] private GameObject SphereObject;
        [SerializeField] private string Path = "Assets/Resources/test.json";

        void Start()
        {
            if (SceneObjectParent == null)
                SceneObjectParent = GameObject.Find("SceneObjects").transform;

            UpdateLists();

            if (File.Exists(Path))
                LoadSceneState();
            else SaveSceneState();
        }

        public void AddSphere()
        {
            GameObject newGo = Instantiate(
                SphereObject, 
                new Vector3(rand.Next(-randRange, randRange), rand.Next(-randRange, randRange), rand.Next(-randRange, randRange)), 
                Quaternion.identity, 
                SceneObjectParent);

            _sceneObjects.Add(newGo);
            _status.Positions.Add(newGo.transform.position);
        }

        public void LoadSceneState()
        {
            ClearScene();

            string jsonstring = ReadString(Path);
            _status = JsonUtility.FromJson<SceneStatus>(jsonstring);
            foreach (Vector3 pos in _status.Positions)
            {
                GameObject newGo = Instantiate(SphereObject, pos, Quaternion.identity, SceneObjectParent);
                _sceneObjects.Add(newGo);
            }
            print("loaded " + jsonstring);
        }

        public void RandomizePositions()
        {
            if (_sceneObjects.Count > 0)
            {
                foreach (GameObject sphereobject in _sceneObjects)
                {
                    sphereobject.transform.position = new Vector3(rand.Next(-randRange, randRange), rand.Next(-randRange, randRange), rand.Next(-randRange, randRange));
                }
            }
        }

        public void SaveSceneState()
        {
            UpdateLists();
            string jsonstring = JsonUtility.ToJson(_status);
            WriteString(Path, jsonstring);
            print("saved " + jsonstring);
        }

        private void UpdateLists()
        {
            _status.Positions.Clear();
            _sceneObjects.Clear();

            {
                if (SceneObjectParent.childCount > 0)
                {
                    for (int i = 0; i < SceneObjectParent.childCount; i++)
                    {
                        GameObject childObject = SceneObjectParent.GetChild(i).gameObject;
                        _sceneObjects.Add(childObject);
                        _status.Positions.Add(childObject.transform.position);
                    }
                }
            }
        }

        public void ClearScene()
        {
            foreach (var go in _sceneObjects)
                Destroy(go);

            _status.Positions.Clear();
            _sceneObjects.Clear();
        }

        // https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
        void WriteString(string path, string jsonstring)
        {
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(jsonstring);
            writer.Close();
        }

        string ReadString(string path)
        {
            StreamReader reader = new StreamReader(path);
            string jsonstring = reader.ReadToEnd();
            reader.Close();

            return jsonstring;
        }
    }

    class SceneStatus
    {
        public List<Vector3> Positions = new List<Vector3>();
    }

}