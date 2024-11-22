﻿namespace ABVInvest.Common.Helpers
{
    public interface IDeserialiser
    {
        Task<IEnumerable<T>> DeserialiseXmlFile<T>(string fileName);
    }
}