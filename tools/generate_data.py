import requests
import json
import os

def generate_data(start_date, end_date, api_key = "DEMO_KEY"):
    url = "https://api.nasa.gov/neo/rest/v1/feed?start_date=%s&end_date=%s&api_key=%s" % (start_date, end_date, api_key)
    response = requests.get(url)
    if (response.ok):
        data = json.loads(response.content)
        return data

with open('NASA_API_key.json') as f:
    api_key = json.load(f)

date_from = "2019-01-02"
date_to = "2019-01-05"
data = generate_data(date_from, date_to, api_key)

out = {'date_range_from': date_from, 'date_range_to': date_to, 'asteroids_by_day': []}

for key, asteroids_list in data['near_earth_objects'].items():
    
    asteroids = []
    for asteroid in asteroids_list:
        asteroids.append({
            'name': asteroid['name'],
            'nasa_jpl_url': asteroid['nasa_jpl_url'],
            'absolute_magnitude_h': asteroid['absolute_magnitude_h'],
            'estimated_diameter_min_meters': "%.3f" % float(asteroid['estimated_diameter']['meters']['estimated_diameter_min']),
            'estimated_diameter_max_meters': "%.3f" % float(asteroid['estimated_diameter']['meters']['estimated_diameter_max']),
            'is_potentially_hazardous_asteroid': asteroid['is_potentially_hazardous_asteroid'],
            'close_approach_date': asteroid['close_approach_data'][0]['close_approach_date_full'],
            'relative_velocity_km_s': "%.3f" % float(asteroid['close_approach_data'][0]['relative_velocity']['kilometers_per_second']),
            'miss_distance_lunar': "%.3f" % float(asteroid['close_approach_data'][0]['miss_distance']['lunar'])
        })
    out['asteroids_by_day'].append({'date': key, 'asteroids': asteroids})

with open(os.path.join('..', 'data', '%s_%s.json' % (date_from, date_to)), "w") as res:
    json.dump(out, res, indent=2)