import urllib.request, json
import ssl
import sys

filename = "latlong-fidelity-2.csv"
apikey = "AIzaSyCOoNIrUYuY6GMRRuveLUyv0z7IGzt95kk"

fidelity = 2
latrange = range(-86, -80, fidelity)
lonrange = range(-180, 180, fidelity)
#latrange = range(-90, 90, fidelity)
#lonrange = range(-180, 180, fidelity)

ctx = ssl.create_default_context()
ctx.check_hostname = False
ctx.verify_mode = ssl.CERT_NONE

with open(filename, 'a') as output:
    for lat in latrange:
        for lon in lonrange:
            #with urllib.request.urlopen("https://maps.googleapis.com/maps/api/geocode/json?latlng="+str(lat)+","+str(lon)+"&result_type=country&key="+apikey, context=ctx) as url:
            try:
                with urllib.request.urlopen("http://api.geonames.org/countryCode?lat="+str(lat)+"&lng="+str(lon)+"&username=demo", context=ctx) as url:
                    '''
                    data = json.loads(url.read().decode())
                    if data['status'] == "OK":
                        name = data['results'][0]['address_components'][0]['short_name']
                        print(str(lat)+", "+str(lon)+" = "+name)
                        output.write(str(lat)+","+str(lon)+","+name+"\n")
                    '''
                    data = url.read().decode()

                    print(str(lat)+", "+str(lon)+" = "+data[:-1])

                    if data[:3] != "ERR":
                        output.write(str(lat)+","+str(lon)+","+data[:2]+"\n")
            except:
                sys.exit()
