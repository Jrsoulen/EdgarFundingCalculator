# EdgarFundingCalculator
Persists data from selected Edgar CIK and provides an API for performing a few calculations on the persisted data.

!! Running this will create an artifact for the SqlLite db, if you're on windows it will go into C:/{User}/AppData/Local/

/***************/
/*** Running ***/

Open in visual studio, set the App.Web as the startup project, and click Run
Or dotnet run the App.Web project

On the first run this will loop across CIKs in the appSettings and populate the SQLLite database
Once that is complete the console will say so and the app will be ready for testing
This step takes about thirty seconds - ridiculously suboptimal for what it is doing.
Not a priority for a one time data seed step, but could easily but cut down to a fraction of that.

/***************/
/***************/


/***************/
/*** Testing ***/

Once running you can open the EdgarFundingCalculator.http and click send request on either request for an immediate integration test
Unit test framework with sample test is also set up

/***************/
/***************/

Open Questions for Product:
	Is Y a vowel?? https://www.merriam-webster.com/grammar/why-y-is-sometimes-a-vowel-usage

If I had to do this again:
	Import all data for selected CIKs, then perform calculations on demand.
	I'm spending too much time on database stuff

Things I'd Do Next: (AKA Things I would have already done)
	Logging!!!
	Refactor to isolate calculations for funding amounts - this might even be better as a scripting language or something
