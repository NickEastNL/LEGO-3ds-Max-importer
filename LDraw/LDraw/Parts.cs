using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Max;

namespace LDraw
{
	/// <summary>
	/// Represents a unique LEGO part (not its copies in the model)
	/// </summary>
	public class UniquePart
	{
		private string _partID;
		private int _amount;
		private bool _exists;
		private string _path;

		/// <summary>
		/// The unique part ID
		/// </summary>
		public string PartID
		{
			get { return _partID; }
			set { _partID = value; }
		}

		/// <summary>
		/// The amount used in the file
		/// </summary>
		public int Amount
		{
			get { return _amount; }
			set { _amount = value; }
		}

		/// <summary>
		/// Whether it exists in the custom library or not
		/// </summary>
		public bool Exists
		{
			get { return _exists; }
			set { _exists = value; }
		}

		/// <summary>
		/// Stores the absolute path to the file
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}
	}

	/// <summary>
	/// Represents an instance of a LEGO part
	/// </summary>
	public class Part
	{
		private string _partID;
		private string _partName;
		private string _colorID;
		private Model _parentModel;
		private IMatrix3 _transform;
		
		/// <summary>
		/// The part ID
		/// </summary>
		public string PartID
		{
			get { return _partID; }
			set { _partID = value; }
		}

		/// <summary>
		/// The unique name used in Max
		/// </summary>
		public string PartName
		{
			get { return _partName; }
			set { _partName = value; }
		}

		/// <summary>
		/// The color ID applied to this part
		/// </summary>
		public string ColorID
		{
			get { return _colorID; }
			set { _colorID = value; }
		}

		/// <summary>
		/// The parent submodel this part belongs to
		/// </summary>
		public Model ParentModel
		{
			get { return _parentModel; }
			set { _parentModel = value; }
		}

		/// <summary>
		/// The transform of this part
		/// </summary>
		public IMatrix3 Transform
		{
			get { return _transform; }
			set { _transform = value; }
		}
	}

	/// <summary>
	/// Represents a unique submodel in a Multi-Part Document
	/// </summary>
	public class Model
	{
		private string _modelName;
		private bool _isMain;
		private List<Part> _parts = new List<Part>();
		private List<Submodel> _submodels = new List<Submodel>();

		/// <summary>
		/// The explicit name of the model
		/// </summary>
		public string ModelName
		{
			get { return _modelName; }
			set { _modelName = value; }
		}

		/// <summary>
		/// Determines if this model is a dependency (it doesn't contain a submodel, but is used in another)
		/// </summary>
		public bool IsMain
		{
			get { return _isMain; }
			set { _isMain = value; }
		}

		/// <summary>
		/// The parts that belong to this submodel
		/// </summary>
		public List<Part> Parts
		{
			get { return _parts; }
			set { _parts = value; }
		}

		/// <summary>
		/// The models that belong to this submodel
		/// </summary>
		public List<Submodel> Submodels
		{
			get { return _submodels; }
			set { _submodels = value; }
		}
	}

	/// <summary>
	/// Represents a submodel instance in a Multi-Part Document
	/// </summary>
	public class Submodel
	{
		private string _submodelName;
		private Model _sourceModel;
		private Model _parentModel;
		private IMatrix3 _transform;
		
		/// <summary>
		/// The unique name of the submodel
		/// </summary>
		public string SubmodelName
		{
			get { return _submodelName; }
			set { _submodelName = value; }
		}

		/// <summary>
		/// The unique model this instance uses
		/// </summary>
		public Model SourceModel
		{
			get { return _sourceModel; }
			set { _sourceModel = value; }
		}

		/// <summary>
		/// The parent submodel this model belongs to
		/// </summary>
		public Model ParentModel
		{
			get { return _parentModel; }
			set { _parentModel = value; }
		}

		/// <summary>
		/// The transform of this submodel
		/// </summary>
		public IMatrix3 Transform
		{
			get { return _transform; }
			set { _transform = value; }
		}
	}
}
