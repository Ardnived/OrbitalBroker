import fiona
from shapely.geometry import asShape, Point, box

filename = "latlong-fidelity-1.csv"

with open(filename, 'w') as output:
    with fiona.open("shapefiles/TM_WORLD_BORDERS-0.3.shp") as fiona_collection:
        records = []
        shapes = []
        boxes = []

        for shapefile_record in fiona_collection:
            shape = asShape(shapefile_record['geometry'])
            shapes.append(shape)

            minx, miny, maxx, maxy = shape.bounds
            boxes.append(box(minx, miny, maxx, maxy))

            records.append(shapefile_record)

        print("Found "+str(len(shapes))+" shapes")
        for lat in range(-90, 91):
            for lon in range(-180, 181):
                point = Point(lon, lat) # longitude, latitude

                for i in range(len(shapes)):
                    if boxes[i].contains(point) and shapes[i].contains(point):
                        properties = records[i]['properties']
                        print(str(lat)+","+str(lon)+" = "+properties['NAME']+" / "+properties['ISO2']+" (tested "+str(i)+" shapes)")
                        output.write(str(lat)+","+str(lon)+","+properties['ISO2']+","+properties['NAME']+"\n")
                        break
