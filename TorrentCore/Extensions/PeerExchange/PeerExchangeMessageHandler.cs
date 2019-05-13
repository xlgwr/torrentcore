﻿// This file is part of TorrentCore.
//   https://torrentcore.org
// Copyright (c) Samuel Fisher.
//
// Licensed under the GNU Lesser General Public License, version 3. See the
// LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using TorrentCore.Extensions.ExtensionProtocol;
using TorrentCore.Transport;
using TorrentCore.Transport.Tcp;

namespace TorrentCore.Extensions.PeerExchange
{
    public class PeerExchangeMessageHandler : IExtensionProtocolMessageHandler
    {
        private readonly ILogger<PeerExchangeMessageHandler> _logger;
        private readonly ITcpTransportProtocol _tcpTransportProtocol;

        public PeerExchangeMessageHandler(ILogger<PeerExchangeMessageHandler> logger, ITcpTransportProtocol tcpTransportProtocol)
        {
            _logger = logger;
            _tcpTransportProtocol = tcpTransportProtocol;
        }

        public IReadOnlyDictionary<string, Func<IExtensionProtocolMessage>> SupportedMessageTypes { get; } = new Dictionary<string, Func<IExtensionProtocolMessage>>
        {
            [PeerExchangeMessage.MessageType] = () => new PeerExchangeMessage(),
        };

        public void PrepareExtensionProtocolHandshake(IPrepareExtensionProtocolHandshakeContext context)
        {
        }

        public void PeerConnected(IExtensionProtocolPeerContext context)
        {
        }

        public void MessageReceived(IExtensionProtocolMessageReceivedContext context)
        {
            var message = context.Message as PeerExchangeMessage;
            if (message == null)
                throw new InvalidOperationException($"Expected a {nameof(PeerExchangeMessage)} but received a {context.Message.GetType().Name}");

            _logger.LogDebug($"{message.Added.Count} peers received from PEX message");

            context.PeersAvailable(message.Added.Select(CreateTransportStream));
        }

        private ITransportStream CreateTransportStream(IPEndPoint endPoint)
        {
            return _tcpTransportProtocol.CreateTransportStream(endPoint.Address, endPoint.Port);
        }
    }
}
