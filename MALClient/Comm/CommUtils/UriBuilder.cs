﻿using System;

namespace MALClient.Comm
{
    public static class UriBuilder
    {
        public static string GetUri(UriType type, IParameters parameters)
        {
            string uri;
            switch (type)
            {
                case UriType.MalListQuery:
                    uri = "http://myanimelist.net/malappinfo.php?";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            uri += parameters.GetParamChain();
            return uri;
        }
    }
}