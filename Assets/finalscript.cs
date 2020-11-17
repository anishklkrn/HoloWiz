
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using ApiAiSDK;
using ApiAiSDK.Model;
using ApiAiSDK.Unity;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using SpeechLib;
using System.Xml;
using UnityEngine.Windows.Speech;

public class finalscript : MonoBehaviour {
    public Animator anim;
    string inputtext;
    DictationRecognizer dictationRecognizer;
    public SpVoice voice;
    public ApiAiUnity apiAiUnity;
    
    void Awake()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += dictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

    }
    private void dictationRecognizer_DictationHypothesis(string text)
    {
        voice.Speak("Listening", SpeechVoiceSpeakFlags.SVSFlagsAsync);
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * add listening animation
         * 
         * 
         * 
         * 
         * 
         */
        //anim.Play("Animation Name", -1, 0f);
        
    }
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        inputtext = text;
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        print("DictationComplete");

        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * add thinking animation
         * 
         * 
         * 
         * 
         * 
         * */
        //anim.Play("Animation name", -1, 0f);
            var text = inputtext;

            Debug.Log(text);

            AIResponse response = apiAiUnity.TextRequest(text);

            if (response != null)
            {
                Debug.Log("Resolved query: " + response.Result.ResolvedQuery);
                var outText = JsonConvert.SerializeObject(response, jsonSettings);
                char[] delimiterChars = { ',', '.', ':', '\t' };
                string[] words = outText.Split(delimiterChars);
                int n = 0;
                foreach (var word in words)
                {
                    n++;
                    string nstr = n.ToString();
                    
                }

                Debug.Log(outText);


                Debug.Log("Result: " + outText);
            /*
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * add answering animation
             * 
             * 
             * 
             * 
             * 
             * 
             * */
            //anim.Play("Answering animation", -1, 0f);
            voice.Volume = 100;
                voice.Rate = 0;
                voice.Speak(words[21], SpeechVoiceSpeakFlags.SVSFlagsAsync);

            }
            else
            {
                Debug.LogError("Response is null");
            }

        
    }

    

    private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
    };
    private readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();







    // Use this for initialization
    IEnumerator Start()
    {
        anim = GetComponent<Animator>();
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * add entry animation
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */
        //anim.Play("Animation name", -1, 0f);
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            throw new NotSupportedException("Microphone using not authorized");
        }

        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) =>
        {
            return true;
        };

        voice = new SpVoice();

        SpObjectTokenCategory tokenCat = new SpObjectTokenCategory();
        tokenCat.SetId(SpeechLib.SpeechStringConstants.SpeechCategoryVoices, false);
        ISpeechObjectTokens tokens = tokenCat.EnumerateTokens(null, null);
        voice.Voice = (tokens.Item(1));

        const string ACCESS_TOKEN = "ecfb1e646b9046089aa2f24660abe490";

        var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);

        apiAiUnity = new ApiAiUnity();
        apiAiUnity.Initialize(config);
        print("API configured");

       // apiAiUnity.OnError += HandleOnError;
       // apiAiUnity.OnResult += HandleOnResult;
    }

   /* private void HandleOnResult(object sender, AIResponseEventArgs e)
    {
        RunInMainThread(() => {
            var aiResponse = e.Response;
            if (aiResponse != null)
            {
                Debug.Log(aiResponse.Result.ResolvedQuery);
                string outText = JsonConvert.SerializeObject(aiResponse, jsonSettings);
                char[] delimiterChars = { '"' };
                string[] words = outText.Split(delimiterChars);
                int n = 0;
                foreach (var word in words)
                {
                    n++;
                    string nstr = n.ToString();
                    print(word + nstr);
                }

                Debug.Log(outText);

                answerTextField.text = words[21];

                voice.Volume = 100;
                voice.Rate = 0;
                voice.Speak(words[21], SpeechVoiceSpeakFlags.SVSFlagsAsync);


            }
            else
            {
                Debug.LogError("Response is null");
            }
        });
      
    }

    private void HandleOnError(object sender, AIErrorEventArgs e)
    {
        throw new NotImplementedException();
    }
    private void RunInMainThread(Action action)
    {
        ExecuteOnMainThread.Enqueue(action);
    }*/

    // Update is called once per frame
    void Update ()
    {
        if (Input.anyKeyDown)
        {
            dictationRecognizer.Start();
        }
        if (apiAiUnity != null)
        {
            apiAiUnity.Update();
        }

        // dispatch stuff on main thread
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }

    }
    /*public void SendText()
    {
        var text = inputtext;

        Debug.Log(text);

        AIResponse response = apiAiUnity.TextRequest(text);

        if (response != null)
        {
            Debug.Log("Resolved query: " + response.Result.ResolvedQuery);
            var outText = JsonConvert.SerializeObject(response, jsonSettings);
            char[] delimiterChars = { ',', '.', ':', '\t' };
            string[] words = outText.Split(delimiterChars);
            int n = 0;
            foreach (var word in words)
            {
                n++;
                string nstr = n.ToString();
                print(word + nstr);
            }

            Debug.Log(outText);


            Debug.Log("Result: " + outText);

            answerTextField.text = words[21];
            voice.Volume = 100;
            voice.Rate = 0;
            voice.Speak(words[21], SpeechVoiceSpeakFlags.SVSFlagsAsync);

        }
        else
        {
            Debug.LogError("Response is null");
        }

    }*/
}
