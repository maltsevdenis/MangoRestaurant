﻿namespace Mango.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(BaseMessage message, string topic);
}