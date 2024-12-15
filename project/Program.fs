
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema
open System.Linq
open System

[<Table("User")>]
type User() =
    [<Key>]
    [<Column("Id")>]
    member val Id: int = 0 with get, set

    [<Column("Name")>]
    member val Name: string = "" with get, set

    [<Column("PhoneNumber")>]
    member val PhoneNumber: int = 0 with get, set
    
    [<Column("Username")>]
    member val Username: string = "" with get, set

    [<Column("Password")>]
    member val Password: string = "" with get, set

    
[<Table("Movie")>]
type Movie() =
    [<Key>]
    [<Column("Id")>]
    member val Id: int = 0 with get, set

    [<Column("MovieName")>]
    member val MovieName: string = "" with get, set

    [<Column("Showtime")>]
    member val Showtime: DateOnly = DateOnly.MinValue with get, set
    

[<Table("Seat")>]
type Seat() =
    [<Key>]
    [<Column("Id")>]
    member val Id: int = 0 with get, set

    [<Column("Row")>]
    member val Row: int = 0 with get, set

    [<Column("Column")>]
    member val Column: int = 0 with get, set
    
    [<Column("isAvailable")>]
    member val isAvailable: bool = true with get, set

    [<Column("MovieId")>]
    member val MovieId: int = 0 with get, set

    
[<Table("Ticket")>]
type Ticket() =
    [<Key>]
    [<Column("Id")>]
    member val Id: int = 0 with get, set

    [<Column("CustomerName")>]
    member val CustomerName: string = "" with get, set

    [<Column("MovieName")>]
    member val MovieName: string = "" with get, set
    
    [<Column("SeatId")>]
    member val SeatId: int = 0 with get, set

    [<Column("Showtime")>]
    member val Showtime: DateOnly = DateOnly.MinValue with get, set




type AppDbContext(options: DbContextOptions<AppDbContext>) =
    inherit DbContext(options)

    override this.OnModelCreating(modelBuilder: ModelBuilder) =
        modelBuilder.Entity<User>() |> ignore
        modelBuilder.Entity<Movie>() |> ignore
        modelBuilder.Entity<Seat>() |> ignore
        modelBuilder.Entity<Ticket>() |> ignore
        
type UserService(dbContext: AppDbContext) =

    member this.AddUser(name: string, phoneNumber: int, username: string, password: string) =
        let user = User(Name = name, PhoneNumber = phoneNumber, Password = password, Username = username)
        dbContext.Add(user) |> ignore
        dbContext.SaveChanges() |> ignore
        "User added successfully!"

    member this.GetAllUsers()=
        dbContext.Set<User>().ToList()|> List.ofSeq

    member this.AddMovie(moviename: string,showtime: DateOnly) =
        let movie =Movie(MovieName=moviename,Showtime=showtime)
        dbContext.Add(movie) |> ignore
        dbContext.SaveChanges() |> ignore
        printfn "Movie added successfully!"

    /////zainab//////
    member this.GetUserByCred(username: string, password: string)=
       let user=dbContext.Set<User>().FirstOrDefault(fun m -> m.Username=username && m.Password=password)
       Option.ofObj user

    member this.GetUserByName(name: string)=
       let user=dbContext.Set<User>().FirstOrDefault(fun u -> u.Name =name)
       Option.ofObj user


       /////zaineb//////

        //////mariam//////
    member this.AddSeat(row: int,column: int, movieId: int)=
        let seat = Seat(Row=row,Column=column,MovieId = movieId)
        dbContext.Add(seat) |> ignore
        dbContext.SaveChanges() |>ignore


    member this.GetMovieById(movieId: int)=
        let movie = dbContext.Set<Movie>().FirstOrDefault( fun m -> m.Id =movieId)
        Option.ofObj movie

    member this.GetAllMovies()=
        dbContext.Set<Movie>().ToList() |> List.ofSeq

    member this.GetAllSeats(movieId: int)=
        dbContext.Set<Seat>().ToList().Where( fun m -> m.MovieId=movieId) |> List.ofSeq

    member this.GetSeat(movieId: int, row: int, column: int)=
       let seat= dbContext.Set<Seat>().FirstOrDefault(fun m -> m.MovieId = movieId && m.Row= row && m.Column=column)
       Option.ofObj seat
       ///mariam////

let optionsBuilder = DbContextOptionsBuilder<AppDbContext>()
optionsBuilder.UseSqlServer("Data Source=ZENOO;Initial Catalog=PL3_Project;Integrated Security=True;Connect Timeout=30;Encrypt=False;
                             TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False") |> ignore
use dbContext = new AppDbContext(optionsBuilder.Options)
let userService = UserService(dbContext)



////////zaineb////////

let login username password =
    let user = userService.GetUserByCred(username, password)
    match user with 
    |Some user -> user.Name
    |None -> ""


//signup
let signup name phonenumber username password=
    if name = "" || username= "" || password = "" then
        "please fill all fields"
    else
    let user = userService.GetUserByCred(username, password)
    match user with 
    |Some user -> "user exist"
    |None ->  match userService.GetUserByName(name)  with
                | Some user -> "this name exist before"
                | None -> userService.AddUser(name,phonenumber,username,password)




// Add a new user
//userService.AddUser("Alice", 1234567890, "password123", "alice")
////userService.AddMovie("awlad_rezk", DateOnly(2024, 12, 10))

/////zaineb/////

/////////////mariam/////////




//check if movie exist
//let movieExist movieId =
//    match userService.GetMovieById(movieId) with
//    |Some movie -> movieId
//    |None ->  0

//adding seats
//let movieid= movieExist 2
//if movieid <> 0 then
//    for i in 1..5 do
//        for j in 1..8 do
//            userService.AddSeat( i, j,movieid)
//            printfn"seat added"
//else
//    printfn"error occured during add a seat"



//display all movies
let displayMovies =
    let movies = userService.GetAllMovies()
    if List.isEmpty movies then
        []
    else
        movies |> List.map(fun m -> (m.Id,m.MovieName,m.Showtime))


// display all seats
let DisplaySeats movieId=
    let seats = userService.GetAllSeats(movieId)
    if (List.isEmpty seats) then
        printfn"there is no seats for this movie"
        []
    else
        let tupleOfSeats =  seats |> List.map(fun seat -> (seat.Row,seat.Column,seat.isAvailable))
        tupleOfSeats
    
//DisplaySeats 2 |> List.iter (fun (row , column) -> printfn"row: %d , column:%d" row column)

//check seat availabilty
let checkAvailablity movieId row column=
    let seat =userService.GetSeat(movieId,row,column)
    match seat with
    | Some Seat -> 
         if Seat.isAvailable=true then
            "this seat is available"
         else
            "this seat is reserved"
    | None -> "there is no seat!"

//checkAvailablity 2 1 1
//login "alice" "password123"
//signup "mostafa" 01128098800 "mostafa" "password123"

//displayMovies


//////////mariam//////////////
