using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 5.0f;
    Vector3 target = new Vector3(0, 1.0f, -6.89f);
    CanvasGroup canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //https://discussions.unity.com/t/how-to-change-the-alpha-of-the-whole-canvas/175843
        //Pour des raisons ben obscur c'est canvas group qui as un alpha. ok.
        canvas = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        //https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Vector3.MoveTowards.html (avec lerp à place.)
        float step = cameraSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.Lerp(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) < 1f)
        {
            canvas.alpha += 0.005f;
            if(canvas.alpha >= 1f)
            {
                if (Vector3.Distance(transform.position, target) <= 0.001f)
                    //C'est fini. Merci camera.
                    Destroy(this);
            }    
        }
    }
}
