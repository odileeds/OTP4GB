# OTP4GB
Open Trip Planner for Great Britain. Journey planning and accessibility mapping for all of GB, or parts of it by Walking, Bicycle, Driving, and Public Transport of all forms except flying.

## Motivation
We want to be able to answer questions like,
* How many more people will be able to get to The Trafford Centre by public transport once the tram extension opens?
* What percentage of the population of Greater Manchester can get to Manchester City Centre within 60 minutes by both public transport and driving?
* What percentage of Leeds City Region's poorest third of areas can access Leeds City Centre within 30 minutes?
* If the UK government builds Northern Powerhouse rail, how will that change the number of people who can reach Bradford City Centre by 9am having left home at 8am?
* Which areas of Sheffield City Region have the poorest access to public transport?
* How many people will lose access to public transport if specific bus routes are cancelled?
* What apprenticeships and further education opportunities can people in every part of Greater Manchester access within 60 minutes of travel?
Using transparent code and open data.

There are many more questions that we want to be able to answer and by releasing the tools that let us answer them we hope that other people will answer them. Our dream is that they will then share their methods and results so that they can be re-used by other areas.

## Output
This project lets you create an OpenTripPlanner instance for any part of Great Britain that you want. Large areas of GB will be extremely slow.

## Limitations
* The Open Trip Planner instance is slow for all but the smallest geographical areas.
* The Open Trip Planner instance is slow for all but the smallest date window of analysis.
* The source files are mostly correct, but not completely correct. They probably give reasonable journey time estimates for normal weekdays, but not outside of those times.

## How could you help?
The above limitations mean that we cannot sensibly run an Open Trip Planner server instance for the whole of GB. Doing so, even with the public transport timetables reduced to a single day, requires a server with 64GB of RAM. Even then, even with dozens of cores dedicated to the server, planning a single journey takes on the order of minutes. I have tried other types of open source server, such as Navitia and R5, but made no significant progress. Any help on this will be appreciated and all the files needed to let you try something are available.

Similarly, any updates to the conversion software that creates GTFS format public transport timetables for the UK would improve the product, and many others.

Lastly, if you are a UK government body, or similar, and you use this work please consider shraing what you've done. It is inefficient for separate local and regional governments around the UK to be repeating identical work on transport accessibility. By sharing what we're doing and working in the open we can do better than this.

## Tools required
* [Open Trip Planner](github.com/opentripplanner/).
* [osmconvert](https://wiki.openstreetmap.org/wiki/Osmconvert) (64-bit version required).
* [gtsf filter](https://github.com/twalcari/gtfs-filter).

## Input data sources
The tool requires three sources of information,
* [OSM maps via GeoFabrik](http://download.geofabrik.de/)
* GB public transport timetables (except for rail) via [Traveline](https://www.travelinedata.org.uk/), converted into GTFS using [TransXChange2GTFS](github.com/danbillingsley/TransXChange2GTFS).
* GB railway transport timetables via [ATOC](http://data.atoc.org/data-download), converted into GTFS using [ATOCCIF2TransXChange](https://github.com/thomasforth/ATOCCIF2TransXChange).

## Relevant links
The [ONS propeR](https://github.com/datasciencecampus/propeR) work assisted us enormously in this project. We have in many ways simply extended their approach for Wales to the whole of Great Britain.

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
