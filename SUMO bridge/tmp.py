import re

file = "2021-09-27-19-54-09\osm.passenger.trips.xml"

with open (file, 'r+' ) as f:
    content = f.read()
    content_new = re.sub(
        'trip id="veh',
        'trip id="',
        content)
    f.seek(0)
    f.write(content_new)
