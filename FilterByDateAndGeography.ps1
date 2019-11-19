# We need to crop the osm.pbf file
# And all of the GTFS public transport files for GB
# And then put them all in one folder, which we then use to run an open trip planner instance.
# The output folder will be called "FilteredOutput"
# The bounding box tool at https://boundingbox.klokantech.com/ is very useful for creating bounding boxes

# These are our example variables and will filter our input to contain only journeys in South Yorkshire and on the 10th of September 2019.
$dateFilterString = "2019-09-10:2019-09-11"
# -4.0148,52.8808,0.4061,54.7738
$minLon = "-1.8354"
$maxLon = "-0.7571"
$minLat = "53.1189"
$maxLat = "53.7405"

$locationString = $minLat + ':' + $minLon + ':' + $maxLat + ':' + $maxLon

# Prepare some folders for output
Remove-Item FilteredOutput -Recurse
mkdir FilteredOutput

# Crop the osm.pbf map of GB to the bounding box
# If you are not using Windows a version of osmconvert on your platform may be available via https://wiki.openstreetmap.org/wiki/Osmconvert
Start-Process -Wait -FilePath "./osmconvert64.exe" -ArgumentList "great-britain-latest.osm.pbf -b=""$minLon,$minLat,$maxLon,$maxLat"" --complete-ways -o=FilteredOutput/gbfiltered.pbf"

# Crop the GB rail timetable file to the specificed date range and bounding box
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar GBRail_GTFS.zip -d $dateFilterString -l $locationString -o GBRail_GTFS_tmp"

# Crop each of GB's regional GTFS public transport files to the specificed date range and bounding box. "12G" can be reduced if there is less RAM available.
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar EA_GTFS.zip -d $dateFilterString -l $locationString -o EA_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar EM_GTFS.zip -d $dateFilterString -l $locationString -o EM_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar L_GTFS.zip -d $dateFilterString -l $locationString -o L_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar NE_GTFS.zip -d $dateFilterString -l $locationString -o NE_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar NW_GTFS.zip -d $dateFilterString -l $locationString -o NW_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar S_GTFS.zip -d $dateFilterString -l $locationString -o S_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar SE_GTFS.zip -d $dateFilterString -l $locationString -o SE_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar W_GTFS.zip -d $dateFilterString -l $locationString -o W_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar WM_GTFS.zip -d $dateFilterString -l $locationString -o WM_GTFS_tmp"
Invoke-Expression "java -Xmx12G -jar gtfs-filter-0.1.jar Y_GTFS.zip -d $dateFilterString -l $locationString -o Y_GTFS_tmp"

# Compress the output folders into zip files (this makes them valid GTFS files).
Compress-archive GBRail_GTFS_tmp\* FilteredOutput/GBRail_GTFS_filtered.zip -update
Compress-archive EA_GTFS_tmp\* FilteredOutput/EA_GTFS_filtered.zip -update
Compress-archive EM_GTFS_tmp\* FilteredOutput/EM_GTFS_filtered.zip -update
Compress-archive L_GTFS_tmp\* FilteredOutput/L_GTFS_filtered.zip -update
Compress-archive NE_GTFS_tmp\* FilteredOutput/NE_GTFS_filtered.zip -update
Compress-archive NW_GTFS_tmp\* FilteredOutput/NW_GTFS_filtered.zip -update
Compress-archive S_GTFS_tmp\* FilteredOutput/S_GTFS_filtered.zip -update
Compress-archive SE_GTFS_tmp\* FilteredOutput/SE_GTFS_filtered.zip -update
Compress-archive SW_GTFS_tmp\* FilteredOutput/SW_GTFS_filtered.zip -update
Compress-archive W_GTFS_tmp\* FilteredOutput/W_GTFS_filtered.zip -update
Compress-archive WM_GTFS_tmp\* FilteredOutput/WM_GTFS_filtered.zip -update
Compress-archive Y_GTFS_tmp\* FilteredOutput/Y_GTFS_filtered.zip -update

# Delete the temporary folders
Remove-Item GBRail_GTFS_tmp -Recurse
Remove-Item EA_GTFS_tmp -Recurse
Remove-Item EM_GTFS_tmp -Recurse
Remove-Item L_GTFS_tmp -Recurse
Remove-Item NE_GTFS_tmp -Recurse
Remove-Item NW_GTFS_tmp -Recurse
Remove-Item S_GTFS_tmp -Recurse
Remove-Item SE_GTFS_tmp -Recurse
Remove-Item SW_GTFS_tmp -Recurse
Remove-Item W_GTFS_tmp -Recurse
Remove-Item WM_GTFS_tmp -Recurse
Remove-Item Y_GTFS_tmp -Recurse