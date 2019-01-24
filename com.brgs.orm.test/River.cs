using System;
namespace com.brgs.orm.test
{
    internal class River
    {
        public string Id { get; set; }
		public string Name { get; set; }
		public string RiverId { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Srs { get; set; }
        public RiverData[] Levels { get; set; }
        public RiverData[] Flow { get; set; }
        public RiverData[] RiverData { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }

		public River() {
		}
    }
    internal class RiverData
    {
        public DateTime DateTime { get; set; }
        public object Value { get; set; }
        public string Flow { get; set; }
        public string Level { get; set; }        
    }
}