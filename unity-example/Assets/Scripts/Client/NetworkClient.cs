﻿// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;

#if CLIENT

// *** NETWORK CLIENT FOR TCP CONNECTIONS WITH THE SERVER ***

public class NetworkClient
{
    private GameClient gameClient;

	private TcpClient client = null;

	private MatchStatusInfo matchStatusInfo = null;

	private bool connectionSucceeded = false;
	public bool ConnectionSucceeded() { return connectionSucceeded; }

    public NetworkClient()
    {
        this.gameClient = new GameClient();
    }

    // Calls the matchmaking client to do matchmaking against the backend and then connects to the game server with TCP
    public IEnumerator DoMatchMakingAndConnect()
	{
		Debug.Log("Request matchmaking...");
        GameObject.FindObjectOfType<UIManager>().SetTextBox("Requesting matchmaking...");
        yield return null;
        var matchMakingRequestInfo = this.gameClient.MatchMaking();
		Debug.Log("TicketId: " + matchMakingRequestInfo.TicketId);

		if (matchMakingRequestInfo != null)
		{
			bool matchmakingDone = false;
			int tries = 0;
			while (!matchmakingDone)
			{
				Debug.Log("Checking match status...");
				GameObject.FindObjectOfType<UIManager>().SetTextBox("Checking match status...");
				yield return null;
				this.matchStatusInfo = this.gameClient.MatchStatus(matchMakingRequestInfo.TicketId);
				if (matchStatusInfo.PlayerSessionId.Equals("NotPlacedYet"))
				{
					Debug.Log("Still waiting for placement");
					GameObject.FindObjectOfType<UIManager>().SetTextBox("Still waiting for placement...");
					yield return new WaitForSeconds(1.0f);
				}
				else
				{
					Debug.Log("Matchmaking done!");
					GameObject.FindObjectOfType<UIManager>().SetTextBox("Matchmaking done! Connecting to server...");
					yield return null;
					matchmakingDone = true;

                    // Matchmaking done, connect to the servers
					Connect();
				}
				tries++;

                // Return null if we failed after 30 tries
				if (tries >= 30)
				{
					GameObject.FindObjectOfType<UIManager>().SetTextBox("Aborting matchmaking, no match done on 30 seconds");
					Debug.Log("Aborting matchmaking, no match done on 30 seconds");
					yield return null;
					break;
				}
				yield return null;
			}
		}
		else
		{
			GameObject.FindObjectOfType<UIManager>().SetTextBox("Matchmaking failed! Not connected.");
			Debug.Log("Matchmaking request failed!");
		}

		yield return null;
	}

    // Called by the client to receive new messages
	public void Update()
	{
		if (client == null) return;
		var messages = NetworkProtocol.Receive(client);
        
		foreach (SimpleMessage msg in messages)
		{
			HandleMessage(msg);
		}
	}

	private bool TryConnect()
	{
		try
		{
			//Connect with matchmaking info
			Debug.Log("Connect..");
			client = new TcpClient(this.matchStatusInfo.IpAddress, this.matchStatusInfo.Port);
            client.NoDelay = true; // Use No Delay to send small messages immediately. UDP should be used for even faster messaging
			Debug.Log("Done");

			// Send the player session ID to server so it can validate the player
            SimpleMessage connectMessage = new SimpleMessage(MessageType.Connect, this.matchStatusInfo.PlayerSessionId);
            this.SendMessage(connectMessage);

			return true;
		}
		catch (ArgumentNullException e)
		{
			Debug.Log(e.Message);
			client = null;
			return false;
		}
		catch (SocketException e) // server not available
		{
			Debug.Log(e.Message);
			client = null;
			return false;
		}
	}

	private void Connect()
	{
		// try to connect to a local server
		if (TryConnect() == false)
		{
			Debug.Log("Failed to connect to server");
			GameObject.FindObjectOfType<UIManager>().SetTextBox("Connection to server failed.");
		}
		else
		{
			//We're ready to play, let the server know
			this.Ready();
			GameObject.FindObjectOfType<UIManager>().SetTextBox("Connected to server");
		}
	}

	// Send ready to play message to server
	public void Ready()
	{
		if (client == null) return;
		this.connectionSucceeded = true;

        // Send READY message to let server know we are ready
        SimpleMessage message = new SimpleMessage(MessageType.Ready);
		try
		{
			NetworkProtocol.Send(client, message);
		}
		catch (SocketException)
		{
			HandleDisconnect();
		}
	}

    // Send serialized binary message to server
    public void SendMessage(SimpleMessage message)
    {
        if (client == null) return;
        try
        {
            NetworkProtocol.Send(client, message);
        }
        catch (SocketException)
        {
            HandleDisconnect();
        }
    }

	// Send disconnect message to server
	public void Disconnect()
	{
		if (client == null) return;
        SimpleMessage message = new SimpleMessage(MessageType.Disconnect);
		try
		{
			NetworkProtocol.Send(client, message);
		}

		finally
		{
			HandleDisconnect();
		}
	}

	// Handle a message received from the server
	private void HandleMessage(SimpleMessage msg)
	{
		// parse message and pass json string to relevant handler for deserialization
		Debug.Log("Message received:" + msg.messageType + ":" + msg.message);

		if (msg.messageType == MessageType.Reject)
			HandleReject();
		else if (msg.messageType == MessageType.Disconnect)
			HandleDisconnect();
		else if (msg.messageType == MessageType.Spawn)
			HandleOtherPlayerSpawned(msg);
		else if (msg.messageType == MessageType.Position)
			HandleOtherPlayerPos(msg);
		else if (msg.messageType == MessageType.PlayerLeft)
			HandleOtherPlayerLeft(msg);
	}

	private void HandleReject()
	{
		NetworkStream stream = client.GetStream();
		stream.Close();
		client.Close();
		client = null;
	}

	private void HandleDisconnect()
	{
		Debug.Log("Got disconnected by server");
		GameObject.FindObjectOfType<UIManager>().SetTextBox("Got disconnected by server");
		NetworkStream stream = client.GetStream();
		stream.Close();
		client.Close();
		client = null;
	}

	private void HandleOtherPlayerSpawned(SimpleMessage message)
	{
		Client.messagesToProcess.Add(message);
	}

	private void HandleOtherPlayerPos(SimpleMessage message)
    {
		Client.messagesToProcess.Add(message);
	}

	private void HandleOtherPlayerLeft(SimpleMessage message)
	{
		Client.messagesToProcess.Add(message);
	}
}

#endif

