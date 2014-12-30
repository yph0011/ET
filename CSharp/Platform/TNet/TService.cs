﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Base;
using Network;

namespace TNet
{
	public class TService: IService
	{
		private IPoller poller = new TPoller();
		private TSocket acceptor;

		private readonly Dictionary<string, TChannel> channels = new Dictionary<string, TChannel>();

		private readonly TimerManager timerManager = new TimerManager();

		public TService(string host, int port)
		{
			this.acceptor = new TSocket(poller);
			this.acceptor.Bind(host, port);
			this.acceptor.Listen(100);
		}

		public void Dispose()
		{
			if (this.poller == null)
			{
				return;
			}
			
			this.acceptor.Dispose();
			this.acceptor = null;
			this.poller = null;
		}

		public void Add(Action action)
		{
			this.poller.Add(action);
		}

		private async void AcceptAsync()
		{
			while (true)
			{
				TSocket newSocket = new TSocket(poller);
				await this.acceptor.AcceptAsync(newSocket);
				TChannel channel = new TChannel(newSocket, this);
				channels[newSocket.RemoteAddress] = channel;
			}
		}

		private async Task<IChannel> ConnectAsync(string host, int port)
		{
			TSocket newSocket = new TSocket(poller);
			await newSocket.ConnectAsync(host, port);
			TChannel channel = new TChannel(newSocket, this);
			channels[newSocket.RemoteAddress] = channel;
			return channel;
		}

		public async Task<IChannel> GetChannel()
		{
			TSocket socket = new TSocket(this.poller);
			await acceptor.AcceptAsync(socket);
			TChannel channel = new TChannel(socket, this);
			channels[channel.RemoteAddress] = channel;
			return channel;
		}

		public void Remove(IChannel channel)
		{
			TChannel tChannel = channel as TChannel;
			if (tChannel == null)
			{
				return;
			}
			this.channels.Remove(channel.RemoteAddress);
			this.timerManager.Remove(tChannel.SendTimer);
		}

		public async Task<IChannel> GetChannel(string host, int port)
		{
			TChannel channel = null;
			if (this.channels.TryGetValue(host + ":" + port, out channel))
			{
				return channel;
			}
			return await ConnectAsync(host, port);
		}

		public async Task<IChannel> GetChannel(string address)
		{
			string[] ss = address.Split(':');
			int port = Convert.ToInt32(ss[1]);
			return await GetChannel(ss[0], port);
		}

		public void Start()
		{
			AcceptAsync();

			while (true)
			{
				poller.Run(1);
				this.timerManager.Refresh();
			}
		}

		internal TimerManager Timer
		{
			get
			{
				return this.timerManager;
			}
		}
	}
}