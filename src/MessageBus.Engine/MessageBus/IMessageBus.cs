﻿using RabbitMQ.Client;

namespace MessageBus.Engine.MessageBus;

internal interface IMessageBus
{
    IModel ConnectQueue(string queue);
    void CreateQueue(IModel model, string queue);
    IModel CreateModel();
}