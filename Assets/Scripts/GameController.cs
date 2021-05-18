using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Wall;
    public GameObject StickNote;
    public GameObject Thread;

    [Space(10)]
    public GameObject TargetFrom;
    public string TagFrom;
    public GameObject TargetTo;
    public string TagTo;

    // Start is called before the first frame update
    void Start()
    {
        LoadGameData();
        StartCoroutine(SaveGameDataCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                Debug.Log("You've selected the " + hit.transform.tag);
                if (hit.transform.tag == Wall.tag)
                {
                    var note = Instantiate(StickNote, hit.point - new Vector3(0, 0, 0.08f), StickNote.transform.rotation);
                    note.GetComponent<ThreadPlacer>().GameController = this;
                }
            }
        }

        if (TargetFrom != null && TargetTo != null && TargetFrom != TargetTo)
        {
            var thread = Instantiate(Thread);
            var threadObj = thread.GetComponent<ThreadObject>();
            threadObj.targetFrom = TargetFrom.transform;
            threadObj.tagFrom = TagFrom;
            threadObj.targetTo = TargetTo.transform;
            threadObj.tagTo = TagTo;
            TargetFrom = TargetTo = null;
        }
    }

    public void DeleteThread(string noteTag)
    {
        var threads = GameObject.FindGameObjectsWithTag(Thread.tag);
        foreach (var thread in threads)
        {
            if (thread.GetComponent<ThreadObject>().tagFrom == noteTag || thread.GetComponent<ThreadObject>().tagTo == noteTag)
            {
                Destroy(thread);
            }
        }
    }

    private IEnumerator SaveGameDataCoroutine()
    {
        while (true)
        {
            //yield return new WaitForSeconds(10);
            //LoadGameData();
            yield return new WaitForSeconds(5);
            SaveGameData();
        }
    }

    private void LoadGameData()
    {
        var data = SaveController.LoadData();
        if (data == null)
        {
            Debug.Log("Saved data wasn't found");
            return;
        }

        var notes = new GameObject[data.NoteDatas.Length];
        // Instantiate Notes
        for (int i = 0; i < data.NoteDatas.Length; i++)
        {
            Vector3 pos = new Vector3();
            SerializedVector3.DeserializedVector3(ref pos, data.NoteDatas[i].NotePosition);
            notes[i] = Instantiate(StickNote, pos, StickNote.transform.rotation);

            notes[i].GetComponent<ThreadPlacer>().GameController = this;
            notes[i].GetComponent<ThreadPlacer>().noteTag = data.NoteDatas[i].Tag;
            notes[i].GetComponentInChildren<NoteRecorder>().SetAudioClipPath(data.NoteDatas[i].AudioClipPath);
        }

        // Instantiate Threads
        for (int i = 0; i < data.ThreadDatas.Length; i++)
        {
            var thread = Instantiate(Thread);
            var threadObj = thread.GetComponent<ThreadObject>();

            for (int j = 0; j < notes.Length; j++)
            {
                if (notes[j].GetComponent<ThreadPlacer>().noteTag == data.ThreadDatas[i].TagFrom)
                {
                    threadObj.targetFrom = notes[j].transform;
                    threadObj.tagFrom = notes[j].GetComponent<ThreadPlacer>().noteTag;
                }
                if (notes[j].GetComponent<ThreadPlacer>().noteTag == data.ThreadDatas[i].TagTo)
                {
                    threadObj.targetTo = notes[j].transform;
                    threadObj.tagTo = notes[j].GetComponent<ThreadPlacer>().noteTag;
                }
            }
        }
    }

    public void SaveGameData()
    {
        var notes = GameObject.FindGameObjectsWithTag(StickNote.tag);

        NoteData[] noteDatas = new NoteData[notes.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            noteDatas[i] = new NoteData();
            noteDatas[i].AudioClipPath = notes[i].GetComponentInChildren<NoteRecorder>().audioClipPath;
            noteDatas[i].NotePosition = new SerializedVector3(notes[i].transform.position);
            noteDatas[i].Tag = notes[i].GetComponent<ThreadPlacer>().noteTag;
        }

        var threads = GameObject.FindGameObjectsWithTag(Thread.tag);
        ThreadData[] threadDatas = new ThreadData[threads.Length];
        for (int i = 0; i < threads.Length; i++)
        {
            threadDatas[i] = new ThreadData
            {
                TagFrom = threads[i].GetComponent<ThreadObject>().tagFrom,
                TagTo = threads[i].GetComponent<ThreadObject>().tagTo
            };
        }

        SaveController.SaveData(noteDatas, threadDatas);
    }
}
