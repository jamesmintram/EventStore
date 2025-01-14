﻿using System;
using EventStore.Core.TransactionLog.Chunks;
using EventStore.Core.TransactionLog.Chunks.TFChunk;
using EventStore.Core.TransactionLog.LogRecords;

namespace EventStore.Core.TransactionLog.Scavenging {
	public class ChunkManagerForExecutor<TStreamId> : IChunkManagerForChunkExecutor<TStreamId, ILogRecord> {
		private readonly TFChunkManager _manager;
		private readonly TFChunkDbConfig _dbConfig;

		public ChunkManagerForExecutor(TFChunkManager manager, TFChunkDbConfig dbConfig) {
			_manager = manager;
			_dbConfig = dbConfig;
		}

		public IChunkWriterForExecutor<TStreamId, ILogRecord> CreateChunkWriter(
			IChunkReaderForExecutor<TStreamId, ILogRecord> sourceChunk) {

			return new ChunkWriterForExecutor<TStreamId>(this, _dbConfig, sourceChunk);
		}

		public IChunkReaderForExecutor<TStreamId, ILogRecord> GetChunkReaderFor(long position) {
			var tfChunk = _manager.GetChunkFor(position);
			return new ChunkReaderForExecutor<TStreamId>(tfChunk);
		}

		public void SwitchChunk(
			TFChunk chunk,
			out string newFileName) {

			var tfChunk = _manager.SwitchChunk(
				chunk: chunk,
				verifyHash: false,
				removeChunksWithGreaterNumbers: false);

			if (tfChunk == null) {
				throw new Exception("Unexpected error: new chunk is null after switch");
			}

			newFileName = tfChunk.FileName;
		}
	}
}
