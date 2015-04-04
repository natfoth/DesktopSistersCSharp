﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Xml.Linq;

namespace GoogleMaps.LocationServices
{
    public class GoogleLocationService : ILocationService
    {
        #region Constants
        const string API_REGION_FROM_LATLONG = "maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        const string API_LATLONG_FROM_ADDRESS = "maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false";
        const string API_DIRECTIONS = "maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false";
        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLocationService"/> class.
        /// </summary>
        /// <param name="useHttps">Indicates whether to call the Google API over HTTPS or not.</param>
        public GoogleLocationService(bool useHttps)
        {
            UseHttps = useHttps;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLocationService"/> class. Default calling the API over regular
        /// HTTP (not HTTPS).
        /// </summary>
        public GoogleLocationService()
            : this(false)
        { }
        #endregion


        #region Properties
        /// <summary>
        /// Gets a value indicating whether to use the Google API over HTTPS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if using the API over HTTPS; otherwise, <c>false</c>.
        /// </value>
        public bool UseHttps { get; private set; }


        private string UrlProtocolPrefix
        {
            get
            {
                if (UseHttps)
                    return "https://";
                else
                    return "http://";
            }
        }


        protected string APIUrlRegionFromLatLong
        {
            get
            {
                return UrlProtocolPrefix + API_REGION_FROM_LATLONG;
            }
        }

        protected string APIUrlLatLongFromAddress
        {
            get
            {
                return UrlProtocolPrefix + API_LATLONG_FROM_ADDRESS;
            }
        }

        protected string APIUrlDirections
        {
            get
            {
                return UrlProtocolPrefix + API_DIRECTIONS;
            }
        }
        #endregion


        #region Public instance methods
        /// <summary>
        /// Translates a Latitude / Longitude into a Region (state) using Google Maps api
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public Region GetRegionFromLatLong(double latitude, double longitude)
        {
            XDocument doc = XDocument.Load(string.Format(APIUrlRegionFromLatLong, latitude, longitude));

            var els = doc.Descendants("result").First().Descendants("address_component").Where(s => s.Descendants("type").First().Value == "administrative_area_level_1").FirstOrDefault();
            if (null != els)
            {
                return new Region() { Name = els.Descendants("long_name").First().Value, ShortCode = els.Descendants("short_name").First().Value };
            }
            return null;
        }


        /// <summary>
        /// Gets the latitude and longitude that belongs to an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        /// <exception cref="System.Net.WebException"></exception>
        public MapPoint GetLatLongFromAddress(string address)
        {
            XDocument doc = XDocument.Load(string.Format(APIUrlLatLongFromAddress, Uri.EscapeDataString(address)));

            string status = doc.Descendants("status").FirstOrDefault().Value;
            if (status == "OVER_QUERY_LIMIT" || status == "REQUEST_DENIED")
            {
                throw new System.Net.WebException("Request Not Authorized or Over QueryLimit");
            }

            var els = doc.Descendants("result").Descendants("geometry").Descendants("location").FirstOrDefault();
            if (null != els)
            {
                var latitude =  ParseUS((els.Nodes().First() as XElement).Value);
                var longitude =  ParseUS((els.Nodes().ElementAt(1) as XElement).Value);
                return new MapPoint() { Latitude = latitude, Longitude = longitude };
            }
            return null;
        }

        /// <summary>
        /// Gets the latitude and longitude that belongs to an address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public MapPoint GetLatLongFromAddress(AddressData address)
        {
            return GetLatLongFromAddress(address.ToString());
        }


        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Directions GetDirections(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the directions.
        /// </summary>
        /// <param name="originAddress">From address.</param>
        /// <param name="destinationAddress">To address.</param>
        /// <returns>The directions</returns>
        public Directions GetDirections(AddressData originAddress, AddressData destinationAddress)
        {
            Directions direction = new Directions();

            XDocument xdoc = XDocument.Load(String.Format(APIUrlDirections,
                Uri.EscapeDataString(originAddress.ToString()),
                Uri.EscapeDataString(destinationAddress.ToString())));

            var status = (from s in xdoc.Descendants("DirectionsResponse").Descendants("status")
                          select s).FirstOrDefault();

            if (status != null && status.Value == "OK")
            {
                direction.StatusCode = Directions.Status.OK;
                var distance = (from d in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg")
                               .Descendants("distance").Descendants("text")
                                select d).LastOrDefault();

                if (distance != null)
                {
                    direction.Distance = distance.Value;
                }

                var duration = (from d in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg")
                               .Descendants("duration").Descendants("text")
                                select d).LastOrDefault();

                if (duration != null)
                {
                    direction.Duration = duration.Value;
                }

                var steps = from s in xdoc.Descendants("DirectionsResponse").Descendants("route").Descendants("leg").Descendants("step")
                            select s;

                foreach (var step in steps)
                {
                    Step directionStep = new Step();

                    directionStep.Instruction = step.Element("html_instructions").Value;
                    directionStep.Distance = step.Descendants("distance").First().Element("text").Value;
                    direction.Steps.Add(directionStep);

                }
                return direction;
            }
            else if (status != null && status.Value != "OK")
            {
                direction.StatusCode = Directions.Status.FAILED;
                return direction;
            }
            else
            {
                throw new Exception("Unable to get Directions from Google");
            }

        }
        #endregion

        double ParseUS(string value)
        {
            return Double.Parse(value, new CultureInfo("en-US"));
        }
    }
}
