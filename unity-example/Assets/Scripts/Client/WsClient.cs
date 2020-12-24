// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AWSSignatureV4_S3_Sample.Signers;
using NativeWebSocket;
using UnityEngine;

#if CLIENT

public class WsClient
{
    private WebSocket websocket = null;

    public async Task Connect()
    {
        var url = Global.wsUrl;

        this.websocket = new WebSocket(url, signedHeaders(url));
        this.websocket.OnOpen += onOpen;
        this.websocket.OnMessage += onMessage;
        this.websocket.OnError += onError;
        this.websocket.OnClose += onClose;

        await this.websocket.Connect();
    }

    public async Task Send(byte[] bytes)
    {
        if (WebSocketState.Open == this.websocket.State)
        {
            await this.websocket.Send(bytes);
        }
    }

    public async Task SendText(string message)
    {
        if (WebSocketState.Open == this.websocket.State)
        {
            await this.websocket.SendText(message);
        }
    }

    public void DispatchMessageQueue()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (null != this.websocket)
        {
            this.websocket.DispatchMessageQueue();
        }
#endif
    }

    private void onOpen()
    {
        Debug.Log("Connection open!");
    }

    private void onMessage(byte[] data)
    {
        var message = Encoding.UTF8.GetString(data);
        Debug.Log($"Received OnMessage! {message}");
    }

    private void onError(string error)
    {
        Debug.Log($"Error! {error}");
    }

    private void onClose(WebSocketCloseCode code)
    {
        Debug.Log($"Connection closed! {code}");
    }

    private Dictionary<string, string> signedHeaders(string url)
    {
        var uri = new Uri(url);
        var signer = new AWS4SignerForAuthorizationHeader
        {
            EndpointUri = uri,
            HttpMethod = "GET",
            Service = "execute-api",
            Region = Global.regionString
        };
        // Extract the query parameters
        var queryParameters = "";
        if (1 < url.Split('?').Length)
        {
            queryParameters = url.Split('?')[1];
        }
        var headers = new Dictionary<string, string>();

        var authorization = signer.ComputeSignature(headers,
                                                    queryParameters,
                                                    AWS4SignerBase.EMPTY_BODY_SHA256,
                                                    Client.cognitoCredentials.AccessKey,
                                                    Client.cognitoCredentials.SecretKey);
        headers.Add("Authorization", authorization);
        headers.Add("x-amz-security-token", Client.cognitoCredentials.Token);

        return headers;
    }

    public async Task Close()
    {
        if (null != this.websocket)
        {
            await this.websocket.Close();
        }
    }
}

#endif
