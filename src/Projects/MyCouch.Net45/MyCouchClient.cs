﻿using System;
using EnsureThat;
using MyCouch.Net;
using MyCouch.Serialization;

namespace MyCouch
{
    public class MyCouchClient : IMyCouchClient
    {
        public IConnection Connection { get; private set; }
        public ISerializer Serializer { get; private set; }
        public IChanges Changes { get; private set; }
        public IAttachments Attachments { get; private set; }
        public IDatabase Database { get; private set; }
        public IDocuments Documents { get; private set; }
        public IEntities Entities { get; protected set; }
        public IViews Views { get; private set; }

        public MyCouchClient(string dbUri) : this(new Uri(dbUri)) { }

        public MyCouchClient(Uri dbUri) : this(new BasicHttpClientConnection(dbUri)) { }

        public MyCouchClient(IConnection connection, MyCouchClientBootstraper bootstraper = null)
        {
            Ensure.That(connection, "connection").IsNotNull();

            Connection = connection;

            bootstraper = bootstraper ?? new MyCouchClientBootstraper();

            Serializer = bootstraper.SerializerFn();
            Changes = bootstraper.ChangesFn(Connection);
            Attachments = bootstraper.AttachmentsFn(Connection);
            Database = bootstraper.DatabasesFn(Connection);
            Documents = bootstraper.DocumentsFn(Connection);
            Entities = bootstraper.EntitiesFn(Connection);
            Views = bootstraper.ViewsFn(Connection);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Connection.Dispose();
            Connection = null;
        }
    }
}