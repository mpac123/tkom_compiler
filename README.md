# TKOM compiler
The project was developed as a part of compilers principles class in Warsaw University of Technology. The goal was to create a language for writing templates for generating HTML files fed with models in JSON format. The language extension is *.mpgl which stands for very creative "my page generator language".

## Example
A valid input input may look as follows:

Model in JSON format:
```
{
  "date": "2019-01-01",
  "asteroids": [
    {
    "name": "(2019 AN)",
    "nasa_jpl_url": "http://ssd.jpl.nasa.gov/sbdb.cgi?sstr=3837557",
    "estimated_diameter_min_meters": "40.230",
    "estimated_diameter_max_meters": "89.958",
    "is_potentially_hazardous_asteroid": false,
    "relative_velocity_km_s": "8.369",
    "miss_distance_lunar": "49.688"
    },
    {
    "name": "(2007 YS56)",
    "nasa_jpl_url": "http://ssd.jpl.nasa.gov/sbdb.cgi?sstr=3398654",
    "estimated_diameter_min_meters": "19.256",
    "estimated_diameter_max_meters": "43.057",
    "is_potentially_hazardous_asteroid": false,
    "relative_velocity_km_s": "6.461",
    "miss_distance_lunar": "46.773"
    }
  ]
}
```
Template in my language (*.mpgl):
```
<:def row(asteroid, color)>
  <tr>
    <td style="color:{color}">{asteroid.name}</td>
    <td><a href="{asteroid.nasa_jpl_url}">Url</a></td>
    <td>
      <:if (asteroid.relative_velocity_km_s > 15.0)>
        <b>{asteroid.relative_velocity_km_s}</b>
      </:if>
      <:else>
        {asteroid.relative_velocity_km_s}
      </:else>
    </td>
    <td>{asteroid.estimated_diameter_min_meters} - {asteroid.estimated_diameter_max_meters}</td>
    <td>{asteroid.miss_distance_lunar}</td>
  </tr>
</:def>

<:def main(model)>
  <header>
    <h1>Asteroids - close approaches to Earth</h1>
    <h2>{model.date}</h2>
  </header>
  <body>
    <div>
    <table>
    <:for (asteroid in model.asteroids)>
      <:if (a.is_potentially_hazardous_asteroid)>
        {row(asteroid, "red")}
      </:if>
      <:else>
        {row(asteroid, "black")}
      </:else>
    </:for>
    </table>
    </div>
  </body>
</:def
```

## Available instructions
 - `<:def function(args)> body </:def>: declare a function. There must be at least one function declares, it must be called `main` and have exactly one argument, which will be initialised with the model
 - `{model.property[index].value}`: returns the value of variable
 - `{function(arg1, arg2)`: calls function and returns its value
 - `<:for value in model.array> content using {value} </:for>`: iterates over an array
 - `<:if expression> output <:/if><:else> another output </:else>`: conditional block. Else part is optional
 
 ## Run the project
 
 ### Commands
 You need to have dotnet runtime installed on your machine. To restore dependencies run:
 In order to run unit tests run:
 ```
 dotnet test
 ```
 To restore and run the project change directory to `./TKOM`.
 Restore the project by running: `dotnet restore`
 
 To run the project run
 ```
 dotnet run [OPTIONS]
 ```
 The options are listed below:
 ```
 -t, --template       Required. Path to the *.mpgl file with the template
 -m, --model          Required. Path to the *.json file with the model
 -o, --output         Required. Path to the output file
 -d, --declaration    Add HTML declaration on the top of the file
 ```
 
 ### Examples 
 
 You can try the project out by running examples!
 
 In `tools/generate_data.py` you can find a script that fetches a very cool [NASA NeoWS API](https://data.nasa.gov/Space-Science/Asteroids-NeoWs-API/73uw-d9i8) for asteroids close approaches information within the given date range. Few generated files are already in the `data` folder. They are compatible with the `examples/nasa_many_days.mpgl` template file. Feel free to give it a try!
