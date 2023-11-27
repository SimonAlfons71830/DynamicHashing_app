using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_Hash.Objects
{
    public class PlotOfLand
    {
        private int _registerNumber;
        private ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) _coordinates; //?? prerobit
        private List<int> _properties;
        private string _description;

        private const int MAX_DESC_LENGTH = 11;
        private const int MAX_PROPERTIES_COUNT = 5;

        /// <summary>
        /// Constructor with input parameters to create object
        /// </summary>
        /// <param name="registerNumber"></param>
        /// <param name="description"></param>
        /// <param name="coordinates"></param>
        /// <param name="properties"></param>
        public PlotOfLand(int registerNumber, string description, ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) coordinates, List<int> properties)
        {
            RegisterNumber = registerNumber;
            Description = EditDescription(description);
            Properties = properties;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Constructor to initialize Instance
        /// </summary>
        public PlotOfLand() : this(0, "", ((0, 0), (0, 0)), new List<int>())
        {
        }

        private string EditDescription(string desc)
        {
            if (desc.Length <= MAX_DESC_LENGTH)
            {
                //will add '*' to the length of 15
                return desc.PadRight(MAX_DESC_LENGTH, '*');
            }
            else
            {
                //will shorten the description
                return desc.Substring(0, MAX_DESC_LENGTH);
            }
        }

        public int RegisterNumber
        {
            get => _registerNumber;
            set => _registerNumber = value;
        }

        public ((double LongitudeStart, double LatitudeStart), (double LongitudeEnd, double LatitudeEnd)) Coordinates
        {
            get => _coordinates;
            set => _coordinates = value;
        }

        public List<int> Properties
        {
            get => _properties;
            set
            {
                // Take the first 6 records from the input list or fill with -1 if not full
                _properties = value.Take(MAX_PROPERTIES_COUNT).Concat(Enumerable.Repeat(-1, MAX_PROPERTIES_COUNT- value.Count)).ToList();
            }
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }



    }
}
