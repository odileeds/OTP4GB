using CsvHelper;
using NetTopologySuite.Algorithm.Locate;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Precision;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Text;

string baseFolder = @"Assets";
string RAMCode = "80G"; // if you're running on a virtual machine (no virtual memory/page disk) this must not exceed the total amount of RAM. 80G = 80 gigabytes.

// Set up up some projections to that we can buffer isochrones in metres
string EPSG23030WKT = File.ReadAllText("ProjectionWKT/EPSG23030.txt"); // this is a pretty good North Europe projection with units in metres that is supported by ProjNet
CoordinateSystemFactory csf = new CoordinateSystemFactory();
CoordinateTransformationFactory trf = new CoordinateTransformationFactory();
ICoordinateTransformation TransformToMetres = trf.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, csf.CreateFromWkt(EPSG23030WKT));
ICoordinateTransformation TransformToDegrees = trf.CreateFromCoordinateSystems(csf.CreateFromWkt(EPSG23030WKT), GeographicCoordinateSystem.WGS84);

// This is a good tool for creating your own bounding boxes https://boundingbox.klokantech.com/
// These are our example variables and will filter our input to contain only journeys in South Yorkshire and on the 10th of September 2019.
// Compiling the full GB transport model takes about 80GB of RAM and about 1 hour.
// Calculations using OTP1.x based on the full GB transport model are extremely slow and may not complete.
// Both the date range and geographical extent of the transport must be filtered for reasonable performance.
string dateFilterString = "2019-09-10:2019-09-11";

double minLon;
double maxLon;
double minLat;
double maxLat;

// These are Greater Manchester boundings
minLon = -2.8086;
maxLon = -1.7759;
minLat = 53.2318;
maxLat = 53.839;

// These are Scotland boundings
minLon = -10;
maxLon = 10;
minLat = 54.6;
maxLat = 80;

// These are GB boundings
minLon = -10;
maxLon = 10;
minLat = 40;
maxLat = 80;

// These are North England Zone of Influence boundings
minLon = -3.7731;
maxLon = 0.3797;
minLat = 52.8258;
maxLat = 56.0872;

// North England and Midlands
maxLon = 0.509896555;
minLon = -3.693384901;
maxLat = 55.811076522;
minLat = 51.805604392;

// North England
maxLon = 0.3028;
minLon = -3.6852;
maxLat = 55.8200;
minLat = 52.8828;

// Bradford
maxLon = -1.4186347996;
minLon = -2.0846809422;
maxLat = 53.9786464325;
minLat = 53.6054851748;


if (Directory.Exists($"{baseFolder}/graphs/filtered"))
{
    Console.WriteLine("A folder of filtered GTFS and OSM files already exists. To filter again, delete this folder.");
}
else
{
    Directory.CreateDirectory($"{baseFolder}/graphs/filtered");

    // We need to crop the osm.pbf file and all of the GTFS public transport files for GB
    // And then put them all in one folder, which we then use to run an open trip planner instance.
    string locationString = $"{(decimal)minLat}:{(decimal)minLon}:{(decimal)maxLat}:{(decimal)maxLon}";

    List<string> FilterCommand = new List<string>();
    FilterCommand.Add($"cd {baseFolder}");

    List<string> TimetableFiles = new List<string>();
    //TimetableFiles.Add("NPR_GTFS.zip"); // see https://github.com/thomasforth/NPRHS2_GTFS_Creator for details on imaginary timetables
    //TimetableFiles.Add("HS2_Manc_GTFS.zip"); // see https://github.com/thomasforth/NPRHS2_GTFS_Creator for details on imaginary timetables
    //TimetableFiles.Add("HS2_Brum_GTFS.zip"); // see https://github.com/thomasforth/NPRHS2_GTFS_Creator for details on imaginary timetables
    //TimetableFiles.Add("HS2_Leeds_GTFS.zip"); // see https://github.com/thomasforth/NPRHS2_GTFS_Creator for details on imaginary timetables

    TimetableFiles.Add("GBRail_GTFS.zip"); // see https://github.com/odileeds/ATOCCIF2GTFS
    TimetableFiles.Add("EA_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("EM_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("L_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("NE_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("NW_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("S_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("SE_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("W_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("WM_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS
    TimetableFiles.Add("Y_GTFS.zip"); // see https://github.com/danbillingsley/TransXChange2GTFS

    // Crop the timetable files to the specified date range and bounding box            
    foreach (string timetablefile in TimetableFiles)
    {
        string expressionToExecute = $"java -Xmx{RAMCode} -jar gtfs-filter-0.1.jar {timetablefile} -d {dateFilterString} -l {locationString} -o {timetablefile}_tmp";
        FilterCommand.Add(expressionToExecute);
    }

    // Crop the osm.pbf map of GB to the bounding box
    // If you are not using Windows a version of osmconvert on your platform may be available via https://wiki.openstreetmap.org/wiki/Osmconvert
    FilterCommand.Add($"osmconvert64.exe great-britain-latest.osm.pbf -b={minLon},{minLat},{maxLon},{maxLat} --complete-ways -o=graphs/filtered/gbfiltered.pbf");

    File.WriteAllLines("filtercode.bat", FilterCommand);
    Process runBatch = Process.Start("filtercode.bat");
    runBatch.WaitForExit();

    foreach (string timetablefile in TimetableFiles)
    {
        if (File.Exists($"{baseFolder}/{timetablefile}_filtered.zip"))
        {
            File.Delete($"{baseFolder}/{timetablefile}_filtered.zip");
        }
        ZipFile.CreateFromDirectory($"{baseFolder}/{timetablefile}_tmp", $"{baseFolder}/graphs/filtered/{timetablefile}_filtered.zip", CompressionLevel.Optimal, false, Encoding.UTF8);
        Directory.Delete($"{baseFolder}/{timetablefile}_tmp", true);
    }

    File.Copy($"{baseFolder}/build-config.json", $"{baseFolder}/graphs/filtered/build-config.json");
    File.Copy($"{baseFolder}/router-config.json", $"{baseFolder}/graphs/filtered/router-config.json");    
}

if (File.Exists($"{baseFolder}/graphs/filtered/graph.obj")) {
    Console.WriteLine("A graph.obj file already exists and will be used. To rebuild the transport graph delete the graph.obj file.");
}
else
{
    List<string> BuildCommand = new List<string>();
    BuildCommand.Add($"cd {baseFolder}");
    // the extra --add-opens code here is needed for otp v1.x to be compatible with newer version of Java (JDK11 and JDK17). JDK8 works fine but is rarely installed on newer machines.
    BuildCommand.Add($"java --add-opens java.base/java.util=ALL-UNNAMED --add-opens java.base/java.io=ALL-UNNAMED -Xmx{RAMCode} -jar otp-1.5.0-shaded.jar --build graphs/filtered");

    File.WriteAllLines("build.bat", BuildCommand);
    Process buildBatch = Process.Start("build.bat");
    buildBatch.WaitForExit();
}

if (OTPServerIsRunning() == false) {
    Console.WriteLine("Open Trip Planner does not seem to be running. Starting Open Trip Planner.");
    List<string> RunCommand = new List<string>();
    RunCommand.Add($"cd {baseFolder}");
    RunCommand.Add($"java --add-opens java.base/java.util=ALL-UNNAMED --add-opens java.base/java.io=ALL-UNNAMED -Xmx{RAMCode} -jar otp-1.5.0-shaded.jar --router filtered --graphs graphs --server");

    System.IO.File.WriteAllLines("run.bat", RunCommand);
    Process runBatch = Process.Start("run.bat");
    // runBatch.WaitForExit();
}

while (OTPServerIsRunning() == false)
{
    Console.WriteLine("Waiting one minute for Open Trip Planner to start");
    Task.Delay(new TimeSpan(0, 1, 0)).GetAwaiter().GetResult();
}

if (OTPServerIsRunning() == true)
{
    Console.WriteLine("Performing analysis with Open Trip Planner.");

    // Load Northern MSOAs
    string NorthernMSOAPath = @"Inputs/MSOACentroids_NorthEngland_RoadSnappedLatLong.csv";
    List<MSOACentroid> NorthernMSOAs = new List<MSOACentroid>();
    using (TextReader textReader = File.OpenText(NorthernMSOAPath))
    {
        CsvReader csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture);
        NorthernMSOAs = csvReader.GetRecords<MSOACentroid>().ToList();
    }

    ConcurrentBag<Place> Origins = new ConcurrentBag<Place>();
    List<Place> Destinations = new List<Place>();

    List<MSOACentroid> MSOAsWithinModelBoundingBox = NorthernMSOAs.Where(x => x.Latitude > minLat && x.Latitude < maxLat && x.Longitude < maxLon && x.Longitude > minLon).ToList();

    foreach (MSOACentroid MSOAC in MSOAsWithinModelBoundingBox)
    {
        Place place = new Place()
        {
            Latitude = MSOAC.Latitude,
            Longitude = MSOAC.Longitude,
            Name = MSOAC.msoa11cd
        };
        Origins.Add(place);
        Destinations.Add(place);
    }

    // Acceptable modes seem to be WALK, TRANSIT, BICYCLE, CAR
    List<string> Modes = new List<string>()
    {
        "WALK",
        "CAR",
        "TRANSIT,WALK",
        "BUS,WALK",
        "BICYCLE"
    };

    //Modes.Remove("CAR");

    if (File.Exists("Errors.txt"))
    {
        File.Delete("Errors.txt");
    }

    string outputfolder = "Isochrones";
    if (Directory.Exists(outputfolder) == false)
    {
        Directory.CreateDirectory(outputfolder);
    }

    ConcurrentBag<TravelTime> TravelTimes = new ConcurrentBag<TravelTime>();
    Stopwatch sw = new Stopwatch();
    sw.Start();
    int destinationcount = 0;

    // Calculate all isochrones from Bradford as an example for debug (you may want to change this)
    double BradfordLat = 53.79798488;
    double BradfordLon = -1.747193775;
    foreach (string modestring in Modes)
    {
        Console.WriteLine($"Calculating 15 minute isochrones up to three hours from Bradford by {modestring}.");
        string cutoffstring = "";
        for (int mins = 15; mins <= 180; mins = mins + 15)
        {
            int seconds = mins * 60;
            cutoffstring += $"&cutoffSec={seconds}";
        }

        string IsochroneRequestURL = $"http://localhost:8080/otp/routers/filtered/isochrone?fromPlace={BradfordLat},{BradfordLon}&mode={modestring}&date=09-10-2019&time=8:00am&maxWalkDistance=25000{cutoffstring}&arriveby=false";

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, IsochroneRequestURL);
        requestMessage.Headers.Add("Accept", "application/json"); // this makes the otp instance return geojson instead of a shapefile
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(60);
        HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();
        if (response.IsSuccessStatusCode == false)
        {
            Console.WriteLine($"Failed during {modestring} calculation. Skipping");
        }
        else
        {
            string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var reader = new NetTopologySuite.IO.GeoJsonReader();
            FeatureCollection IsochronesForAllMinutes = reader.Read<FeatureCollection>(responseString);

            var writer = new NetTopologySuite.IO.GeoJsonWriter();
            writer.SerializerSettings.Formatting = Formatting.None;
            File.WriteAllText($"BradfordIsochrones_{modestring}.geojson", writer.Write(IsochronesForAllMinutes));
        }
    }

    Parallel.ForEach(Destinations, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, (destination) =>
   {
       int MaxTime = 90;
       foreach (string modesstring in Modes)
       {
           Console.WriteLine($"Calculating isochrones by {modesstring} to {destination.Name}.");

           if (modesstring == "CAR")
           {
                // Drive isochrones can be very slow to calculate on large networks so we may want to limit them
                MaxTime = 60;
           }

           string cutoffstring = "";
           for (int mins = 1; mins <= MaxTime; mins++)
           {
               int seconds = mins * 60;
               cutoffstring += $"&cutoffSec={seconds}";
           }
           string URL = $"http://localhost:8080/otp/routers/filtered/isochrone?fromPlace={destination.Latitude},{destination.Longitude}&mode={modesstring}&date=09-10-2019&time=8:30am&maxWalkDistance=2500{cutoffstring}&arriveby=true";

           HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, URL);
           requestMessage.Headers.Add("Accept", "application/json"); // this makes the otp instance return geojson instead of a shapefile
            HttpClient client = new HttpClient();
           client.Timeout = TimeSpan.FromMinutes(10);
           HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();
           if (response.IsSuccessStatusCode == false)
           {
               Console.WriteLine($"Failed during {modesstring} calculation. Skipping");
           }
           else
           {
               string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

               var reader = new NetTopologySuite.IO.GeoJsonReader();

               FeatureCollection IsochronesForAllMinutes = reader.Read<FeatureCollection>(responseString);
               IFeature IsochroneForMaxTime = IsochronesForAllMinutes.Where(x => (long)x.Attributes["time"] == 60 * MaxTime).FirstOrDefault();
               if (IsochroneForMaxTime != null && IsochroneForMaxTime.Geometry == null)
               {
                   Console.WriteLine($"Problem with {modesstring} routing to {destination.Name}.");
                   File.AppendAllLines("Errors.txt", new string[] { $"Problem with {modesstring} routing to {destination.Name}." });
               }
               else
               {
                   List<Place> AllPossibleOrigins = new List<Place>(Origins.Where(x => IsochroneForMaxTime.Geometry.EnvelopeInternal.Contains(new Point(new Coordinate() { Y = x.Latitude, X = x.Longitude }).EnvelopeInternal)));
                   ConcurrentBag<Place> OriginsStillToSearchFor = new ConcurrentBag<Place>(AllPossibleOrigins);
                   ConcurrentBag<Place> OriginsStillToSearchForTemp = new ConcurrentBag<Place>();

                   for (int minutes = 1; minutes <= MaxTime; minutes++)
                   {
                       IFeature IsochroneForThisMinute = IsochronesForAllMinutes.Where(x => (long)x.Attributes["time"] == minutes * 60).First();

                       if (IsochroneForThisMinute.Geometry == null)
                       {
                            //Console.WriteLine("Geometry was null.");
                        }
                       else
                       {
                            // Add in projection round-trip here to buffer at 50m                                        
                            Geometry IsochroneInMetres = Transform(IsochroneForThisMinute.Geometry, (MathTransform)TransformToMetres.MathTransform);
                           Geometry BufferedIsochroneInMetres = new BufferOp(IsochroneInMetres).GetResultGeometry(100);
                           Geometry BufferedIsochroneInDegrees = Transform(BufferedIsochroneInMetres, (MathTransform)TransformToDegrees.MathTransform);

                           BufferedIsochroneInDegrees = GeometryPrecisionReducer.Reduce(BufferedIsochroneInDegrees, new PrecisionModel(10000));

                           if (minutes % 15 == 0)
                           {
                               var writer = new NetTopologySuite.IO.GeoJsonWriter();
                               writer.SerializerSettings.Formatting = Formatting.None;
                               File.WriteAllText($"{outputfolder}/Buffered100m_IsochroneBy_{modesstring}_ToWorkplaceZone_{destination.Name}_ToArriveBy_0830am_20191009_within_{minutes}minutes.geojson", writer.Write(BufferedIsochroneInDegrees));
                           }

                            // This is much quicker than other methods because it indexes the geometry for the interior/exterior test
                            IndexedPointInAreaLocator ipal = new IndexedPointInAreaLocator(BufferedIsochroneInDegrees);

                           int foundcount = AllPossibleOrigins.Count - OriginsStillToSearchFor.Count;
                           foreach (var origin in OriginsStillToSearchFor)
                           {
                               Location loc = ipal.Locate(new Coordinate() { Y = origin.Latitude, X = origin.Longitude });
                               if (loc == Location.Interior)
                               {
                                   TravelTime travelTime = new TravelTime()
                                   {
                                       OriginName = origin.Name,
                                       OriginLatitute = origin.Latitude,
                                       OriginLongitude = origin.Longitude,
                                       DestinationName = destination.Name,
                                       DestinationLatitute = destination.Latitude,
                                       DestinationLongitude = destination.Longitude,
                                       Mode = modesstring,
                                       Minutes = minutes
                                   };
                                   TravelTimes.Add(travelTime);
                               }
                               else
                               {
                                   OriginsStillToSearchForTemp.Add(origin);
                               }
                           }
                           OriginsStillToSearchFor = new ConcurrentBag<Place>(OriginsStillToSearchForTemp);
                           OriginsStillToSearchForTemp.Clear();
                       }
                   }
               }
           }
       }
       Console.WriteLine($"Completed {destinationcount++} of {Destinations.Count} in {Math.Round(sw.Elapsed.TotalMinutes, 0)} minutes. Estimated time left is {Math.Round((Destinations.Count * sw.Elapsed.TotalHours / destinationcount) - sw.Elapsed.TotalHours, 0)} hours.");
   });

    // Write out results
    using (TextWriter textWriter = File.CreateText(@"MSOAtoMSOATravelTimeMatrix_ToArriveBy_0830am_20191009.csv"))
    {
        CsvWriter CSVwriter = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
        CSVwriter.WriteRecords(TravelTimes);
    }
}

static bool OTPServerIsRunning()
{
    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/otp/routers/filtered/");
    HttpClient client = new HttpClient();
    try
    {
        HttpResponseMessage response = client.SendAsync(requestMessage).GetAwaiter().GetResult();
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}

static Geometry Transform(Geometry geom, MathTransform transform)
{
    geom = geom.Copy();
    geom.Apply(new MTF(transform));
    return geom;
}
sealed class MTF : NetTopologySuite.Geometries.ICoordinateSequenceFilter
{
    private readonly MathTransform _mathTransform;
    public MTF(MathTransform mathTransform) => _mathTransform = mathTransform;
    public bool Done => false;
    public bool GeometryChanged => true;
    public void Filter(CoordinateSequence seq, int i)
    {
        double x = seq.GetX(i);
        double y = seq.GetY(i);
        double[] transformed = _mathTransform.Transform(new double[] { x, y });
        seq.SetX(i, transformed[0]);
        seq.SetY(i, transformed[1]);
    }
}

public class MSOACentroid
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string msoa11cd { get; set; }
    public string msoa11nm { get; set; }
}
public class TravelTime
{
    public string OriginName { get; set; }
    public double OriginLatitute { get; set; }
    public double OriginLongitude { get; set; }
    public string DestinationName { get; set; }
    public double DestinationLatitute { get; set; }
    public double DestinationLongitude { get; set; }
    public string Mode { get; set; }
    public int Minutes { get; set; }
}
public class Place
{
    public string Name { get; set; }
    public string masterpc { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}