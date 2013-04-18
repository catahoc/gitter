﻿namespace gitter.Git
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public sealed class RevisionReferencesCollection : IEnumerable<Reference>
	{
		#region Events

		public event EventHandler Changed;

		private void OnChanged()
		{
			var handler = Changed;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private readonly SortedDictionary<string, Reference> _container;

		#endregion

		#region .ctor

		internal RevisionReferencesCollection()
		{
			_container = new SortedDictionary<string, Reference>();
		}

		#endregion

		#region Properties

		public object SyncRoot
		{
			get { return _container; }
		}

		public int Count
		{
			get
			{
				lock(SyncRoot)
				{
					return _container.Count;
				}
			}
		}

		public Reference this[string name]
		{
			get
			{
				lock(SyncRoot)
				{
					return _container[name];
				}
			}
		}

		#endregion

		#region Public Methods

		public bool Contains(string referenceName)
		{
			lock(SyncRoot)
			{
				return _container.ContainsKey(referenceName);
			}
		}

		public bool Contains(Reference reference)
		{
			if(reference == null) return false;

			lock(SyncRoot)
			{
				return _container.ContainsKey(reference.Name);
			}
		}

		public IList<Branch> GetBranches()
		{
			lock(SyncRoot)
			{
				if(_container.Count == 0) return new Branch[0];
				var list = new List<Branch>(_container.Count);
				foreach(var reference in _container.Values)
				{
					var branch = reference as Branch;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<RemoteBranch> GetRemoteBranches()
		{
			lock(SyncRoot)
			{
				if(_container.Count == 0) return new RemoteBranch[0];
				var list = new List<RemoteBranch>(_container.Count);
				foreach(var reference in _container.Values)
				{
					var branch = reference as RemoteBranch;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<BranchBase> GetAllBranches()
		{
			lock(SyncRoot)
			{
				if(_container.Count == 0) return new BranchBase[0];
				var list = new List<BranchBase>(_container.Count);
				foreach(var reference in _container.Values)
				{
					var branch = reference as BranchBase;
					if(branch != null) list.Add(branch);
				}
				return list;
			}
		}

		public IList<Tag> GetTags()
		{
			lock(SyncRoot)
			{
				if(_container.Count == 0) return new Tag[0];
				var list = new List<Tag>(_container.Count);
				foreach(var reference in _container.Values)
				{
					var tag = reference as Tag;
					if(tag != null) list.Add(tag);
				}
				return list;
			}
		}

		#endregion

		#region Internal Methods

		internal void Remove(string reference)
		{
			Assert.IsNotNull(reference);

			bool removed;
			lock(SyncRoot)
			{
				removed = _container.Remove(reference);
			}
			if(removed) OnChanged();
		}

		internal void Remove(Reference reference)
		{
			Assert.IsNotNull(reference);

			bool removed;
			lock(SyncRoot)
			{
				removed = _container.Remove(reference.FullName);
			}
			if(removed) OnChanged();
		}

		internal void Rename(string oldName, Reference reference)
		{
			Assert.IsNeitherNullNorWhitespace(oldName);
			Verify.Argument.IsNotNull(reference, "reference");

			lock(SyncRoot)
			{
				_container.Remove(oldName);
				_container.Add(reference.FullName, reference);
			}
			OnChanged();
		}

		internal void Add(Reference reference)
		{
			Verify.Argument.IsNotNull(reference, "reference");

			lock(SyncRoot)
			{
				_container.Add(reference.FullName, reference);
			}
			OnChanged();
		}

		#endregion

		#region IEnumerable<Reference>

		public SortedDictionary<string, Reference>.ValueCollection.Enumerator GetEnumerator()
		{
			return _container.Values.GetEnumerator();
		}

		IEnumerator<Reference> IEnumerable<Reference>.GetEnumerator()
		{
			return _container.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _container.Values.GetEnumerator();
		}

		#endregion
	}
}