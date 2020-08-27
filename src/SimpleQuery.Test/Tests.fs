module Tests

open System
open System.IO
open Microsoft.Data.SqlClient
open Dapper
open Xunit

type Table = {
    ID: int
    Name: string
    NonNullName: string
}

[<Fact>]
let ``My test`` () =

    let builder = new SqlConnectionStringBuilder ()
    builder.DataSource <- @"(LocalDB)\MSSQLLocalDB"
    builder.AttachDBFilename <- @"|DataDirectory|\SampleDatabase.mdf"
    builder.InitialCatalog <- @"Table"
    builder.ConnectTimeout <- 5
    let connectionString = builder.ToString()
    

    Dapper.DefaultTypeMap.MatchNamesWithUnderscores <- true
    try
        use connection = new SqlConnection(connectionString)
        connection.Open()
        let xs = 
            async {
                let! rows = connection.QueryAsync<Table>(sql="SELECT * FROM Table") |> Async.AwaitTask
                return rows
            } |> Async.RunSynchronously

        xs |> Seq.iter (fun row -> printfn "%d / %s / %s" row.ID row.Name row.NonNullName)
        Assert.True(true)
    with
        e -> Assert.True(false)
