# OTP4GB
Open Trip Planner for Great Britain. Journey planning and accessibility mapping for all of GB, or any part of it, by Walking, Bicycle, Driving, Bus, and Public Transport of all forms except flying.

## This won't work
This is just the code portion of the project. [Download a working solution including the required Assets here](https://www.tomforth.co.uk/OTP4GB/OTP4GB.zip), unzip the download, and open in Visual Studio 2022 (or newer).

## Requirements
This project works on Windows. You'll need Visual Studio 2022 or newer. Then just open the `*.sln` file. You'll need a Java runtime installed, all testing was done in the ([Microsoft builds of OpenJDK 17](https://docs.microsoft.com/en-gb/java/openjdk/download).

You'll need a lot of RAM. 32GB should be sufficient to compile a transport model for an area about the size of Yorkshire. 64GB should be sufficient to compile a transport model for the whole of North England, or for London.

The project could be made to work on Linux or Mac easily with some adaptations, but I do not yet have time to do that. If you are looking for a well-documented and cross-platform guide to running Open Trip Planner you would do well to start with the [OpenTripPlanner for R](https://docs.ropensci.org/opentripplanner/) project.

## Defaults
By default the solution will build a graph for a square of about 15 miles by 15 miles centred on Bradford. It will then calculate travel time isochrones by five methods, Walking, Cycling, Bus, Driving, and Public Transport. It will then calculate an MSOA to MSOA travel time matrix. This should take two to four hours depending on your computer.

If the defaults work you have the basis for further analysis.

## Previous versions
A previous versin of this repository included a Docker image. This has been removed. We are happy to provide guidance on running Open Trip Planner within Docker if you want.

## Alternative trip planners
This project uses Open Trip Planner v1.5. For journey planning this has been superceded by the much faster Open Trip Planner v2.x. For road journeys GraphHopper is excellent, but it's public transport calculations are slow for long journeys.

We use Open Trip Planner v1.5 because it uniquely has the ability to create reliable traveltime isochrones. This allows the bulk processing acceleration needed to create the MSOA to MSOA travel time matrices efficiently.

## Motivation
We want to be able to answer questions like,
* How many more people will be able to get to The Trafford Centre by public transport once the tram extension opens?
* What percentage of the population of Greater Manchester can get to Manchester City Centre within 60 minutes by both public transport and driving?
* What percentage of Leeds City Region's poorest third of areas can access Leeds City Centre within 30 minutes?
* If the UK government builds Northern Powerhouse rail, how will that change the number of people who can reach Bradford City Centre by 9am having left home at 8am?
* Which areas of Sheffield City Region have the best access to public transport?
* How many people will lose access to public transport if specific bus routes are cancelled?
* What apprenticeships and further education opportunities can people in every part of Greater Manchester access within 60 minutes of travel?
And we want to do it using transparent code and open data.

There are many more questions that we know other people want to answer and by releasing the tools that let us answer our questions we hope that other people will answer theirs. Our dream is that they will then share their methods and results so that they can be re-used elsewhere.

## Output
This project lets you create an OpenTripPlanner instance for any part of Great Britain. Large areas of GB will be extremely slow.

## Limitations
* The Open Trip Planner instance is slow for all but the smallest geographical areas.
* The Open Trip Planner instance is slow for all but the smallest date window of analysis.
* The source files are mostly correct, but not completely correct. They probably give reasonable journey time estimates for normal weekdays, but not outside of those times.

## Other software used in this project
* [Open Trip Planner](github.com/opentripplanner/). For all of this project I have used version 1.5.0.
* [osmconvert](https://wiki.openstreetmap.org/wiki/Osmconvert) (64-bit version required)(optional). To crop the Open Street Map files to reduce the OTP graph size and speed up journey planning.
* [gtsf filter](https://github.com/twalcari/gtfs-filter) (optional). To crop both the time and geographical extent of the input public transport timetables to reduce the OTP graph size and speed up journey planning.

## Input data sources
The tool uses data from three sources,
* [OSM maps via GeoFabrik](http://download.geofabrik.de/).
* GB public transport timetables (except for rail) via [Traveline](https://www.travelinedata.org.uk/), converted into GTFS using [TransXChange2GTFS](https://github.com/danbillingsley/TransXChange2GTFS).
* GB railway transport timetables via [ATOC](http://data.atoc.org/data-download), converted into GTFS using [ATOCCIF2TransXChange](https://github.com/thomasforth/ATOCCIF2TransXChange).

## Important files
In addition to OSM maps and GTFS timetables two configuration files are essential to making this work properly,
* build-config.json (this contains only `"platformEntriesLinking": true`. Without this the router fails to link streets with train platforms and most railway stations are unusable. The `"osmWayPropertySet": "uk"` property lets the router know that in the UK (as opposed to the USA) all roads except motorways are safe to walk along even if Open Street Map does not have an explicit pavement added.
* router-config.json (this must contain as a mininum `"routingDefaults": {"driveOnRight": false}` because in the UK we drive on the left.) Other possible contents are explained in the [OTP documentation on configuration options](http://docs.opentripplanner.org/en/latest/Configuration/).

## Running it on Azure Web apps
I haven't yet got this to work, but there seem to be good tutorials about how to run a Grizzly Server on Azure, and I just need to follow them.

## Relevant links
The [ONS propeR](https://github.com/datasciencecampus/propeR) work assisted us enormously in this project. We have in many ways simply extended their approach for Wales to the whole of Great Britain. We were unable to get their recommended GB railway timetable conversion tool to work and so have written our own.
Without [Marcus Young's work documenting Open Trip Planner](https://github.com/marcusyoung/otp-tutorial) and writing excellent tutorials this work would have been impossible.

## Support
The following organisations, and many more who I haven't yet written down, have assisted with this work by funding ODILeeds, or by funding projects done by ODILeeds and allowing the outputs to be relased openly, or by improving software that this project relies on,
* ODILeeds and its sponsors (especially those explicitly named below).
* Transport for The West Midlands.
* The Greater Manchester Combined Authority.
* Sheffield City Region.
* Transport for the North.
* West Yorkshire Combined Authority.
* Leeds City Council.
* Bradford Metropoligtan District Council.
* Calderdale Metropolitan Borough Council.
* The Data Science Campus of The Office for National Statistics.

More details on their involvement will come when I get time.
