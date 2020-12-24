using System;
using System.Threading.Tasks;
using UnityEngine;
using LitJson;

#if CLIENT

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void signup()
    {
        try
        {
            var id = -1;
            var name = "jinus7542";
            var query = $"signup(id: {id}, name: \"{name}\")" +
                        $"{{ id, name }}";
            var data = Task.Run(() => GraphClient.Mutation(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                //Debug.Log(data.Result[1]);
                var error = JsonMapper.ToObject(data.Result[1]);
                Debug.Log((string)error["extensions"]["code"]);
                return;
            }
            Debug.Log(data.Result[1]);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void signin()
    {
        try
        {
            var id = 1111;
            var query = $"signin(id: {id})" +
                        $"{{ id, name }}";
            var data = Task.Run(() => GraphClient.Query(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                //Debug.Log(data.Result[1]);
                var error = JsonMapper.ToObject(data.Result[1]);
                Debug.Log((string)error["extensions"]["code"]);
                return;
            }
            Debug.Log(data.Result[1]);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void signinout()
    {
        try
        {
            var id = 1111;
            var query = $"signin(id: {id})" +
                        $"{{ id }}" +
                        $"signout(id: {id})" +
                        $"{{ name }}";
            var data = Task.Run(() => GraphClient.Query(Global.restUrl, query));
            if ("errors" == data.Result[0]) // TODO:: error handling
            {
                for (var i = 1; i < data.Result.Count; i++)
                {
                    //Debug.Log(data.Result[1]);
                    var error = JsonMapper.ToObject(data.Result[i]);
                    Debug.Log((string)error["extensions"]["code"]);
                }
                return;
            }
            for (var i = 1; i < data.Result.Count; i++)
            {
                Debug.Log(data.Result[i]);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 150, 120), "GraphQL Test");

        if (GUI.Button(new Rect(20, 40, 120, 20), "Signup"))
        {
            signup();
        }
        if (GUI.Button(new Rect(20, 70, 120, 20), "Signin"))
        {
            signin();
        }
        if (GUI.Button(new Rect(20, 100, 120, 20), "Signin & Signout"))
        {
            signinout();
        }
    }
}

#endif
