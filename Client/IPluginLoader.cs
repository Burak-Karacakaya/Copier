﻿namespace Client
{
    public interface IPluginLoader
    {
        void Subscribe(IPreCopyEventBroadcaster pre, IPostCopyEventBroadcaster post);
    }
}