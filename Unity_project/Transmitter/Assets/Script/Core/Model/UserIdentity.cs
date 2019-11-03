﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transmitter.Net.Model
{
	public class UserIdentity
	{
		public string IP
		{
			get
			{
				return ip;
			}
		}

		string ip;

		public string Token
		{
			get
			{
				return token;
			}
		}

		string token;
	}
}