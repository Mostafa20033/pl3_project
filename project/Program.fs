
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

    member this.GetUserByCred(username: string, password: string)=
        let user=dbContext.Set<User>().FirstOrDefault(fun m -> m.Username=username && m.Password=password)
        Option.ofObj user

    member this.GetUserByName(name: string)=
        let user=dbContext.Set<User>().FirstOrDefault(fun u -> u.Name =name)
        Option.ofObj user

    member this.AddSeat(row: int,column: int, movieId: int)=
        let seat = Seat(Row=row,Column=column,MovieId = movieId)
        dbContext.Add(seat) |> ignore
        dbContext.SaveChanges() |>ignore


    member this.GetMovieById(movieId: int)=
        let movie = dbContext.Set<Movie>().FirstOrDefault( fun m -> m.Id =movieId)
        Option.ofObj movie

    member this.AddMovie(moviename: string,showtime: DateOnly) =
        let movie =Movie(MovieName=moviename,Showtime=showtime)
        dbContext.Add(movie) |> ignore
        dbContext.SaveChanges() |> ignore
        printfn "Movie added successfully!"

    member this.GetAllMovies()=
        dbContext.Set<Movie>().ToList() |> List.ofSeq

    member this.GetAllSeats(movieId: int)=
        dbContext.Set<Seat>().ToList().Where( fun m -> m.MovieId=movieId) |> List.ofSeq

    member this.GetSeat(movieId: int, row: int, column: int)=
       let seat= dbContext.Set<Seat>().FirstOrDefault(fun m -> m.MovieId = movieId && m.Row= row && m.Column=column)
       Option.ofObj seat

    member this.BookSeat(movieId: int, row: int, column: int)=
       let seat= dbContext.Set<Seat>().FirstOrDefault(fun m -> m.MovieId = movieId && m.Row= row && m.Column=column)
       match Option.ofObj seat with
       | Some newSeat ->
                newSeat.isAvailable <- false
                dbContext.SaveChanges()

    member this.BookCancle(movieId: int, row: int, column: int)=
       let seat= dbContext.Set<Seat>().FirstOrDefault(fun m -> m.MovieId = movieId && m.Row= row && m.Column=column)
       match Option.ofObj seat with
       | Some newSeat ->
                newSeat.isAvailable <- true
                dbContext.SaveChanges()


    member this.saveTicket(username:string,movieName:string,showTime:DateOnly,seatId:int)=
       let seat =Ticket(CustomerName=username,MovieName=movieName,Showtime=showTime,SeatId=seatId)
       dbContext.Add(seat) |>ignore
       dbContext.SaveChanges() |> ignore
       "ticket Saved"

    member this.getTicket(username:string,seatId:int)=
        let seat=dbContext.Set<Ticket>().FirstOrDefault(fun t -> t.CustomerName=username && t.SeatId=seatId)
        Option.ofObj seat

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



/////amr/////////

//generate the movie ticket
let generateTicket (username: string) (showtime: DateOnly) (movieName: string) (seatRow: int) (seatColumn: int) =
    let fileName = $"{username}_{seatRow}_{seatColumn}.txt"
    let time =showtime.ToString("yyyy-MM-dd")
    let content = 
        $"Username: {username}\n" +
        $"Showtime: {time}\n" +
        $"Movie Name: {movieName}\n" +
        $"Seat Row: {seatRow}\n" +
        $"Seat Column: {seatColumn}\n"

    File.WriteAllText("E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/files/"+fileName, content)
    printfn "File generated: %s" fileName

//generateTicket amr DateOnly(2024,12,10) enemy 1 1

// seat booking
let bookSeat userName movieId row column=
    let mutable movieName =""
    let mutable movieTime = DateOnly(2024, 12, 10)
    let seat =userService.GetSeat(movieId,row,column)
    match userService.GetMovieById(movieId) with
    | Some movie -> 
            movieName <- movie.MovieName
            movieTime <- movie.Showtime
    match seat with
    | Some Seat -> 
         if Seat.isAvailable=true then
            userService.BookSeat(movieId,row,column) |> ignore
            generateTicket userName movieTime movieName Seat.Row Seat.Column
            userService.saveTicket(userName,movieName,movieTime,Seat.Id) |>ignore
            "the seat has been booked successfully"
         else
            "this seat is reserved"
    | None -> "there is no seat!"

//bookSeat "mostafa" 2 2 2

let checkTicket movieId username row column =
    let seat =userService.GetSeat(movieId,row,column)
    match seat with
    | Some Seat -> 
        let ticket = userService.getTicket(username,Seat.Id)
        match ticket with
        |Some ticket -> true
        |None ->false
    |None -> false


//checkTicket 2 amr 1 1

let bookCancle movieId username row column=
     userService.BookCancle(movieId,row,column) |> ignore
     "book Cancled"
  

//bookCancle 2 1 1

////// amr /////////







/////////zanon////////////////



//////////////////////////////////////GUI//////////////////////////
//////////////////////////////////////GUI//////////////////////////
//////////////////////////////////////GUI//////////////////////////
//////////////////////////////////////GUI//////////////////////////



let createTextBox (x: int) (y: int) =
    let tb = new TextBox()
    tb.Location <- Point(x, y)
    tb.Width <- 100
    tb

let createLabel (text: string) (x: int) (y: int) =
    let lbl = new Label()
    lbl.Text <- text
    lbl.Location <- Point(x, y)
    lbl.Width <- 100
    lbl

let getIntValue ( value : string) =
        try
            int value  
        with
        | :? System.FormatException -> 0  

       
let createBackground (form: Form) =
    form.Paint.Add(fun e ->
        let g = e.Graphics
        let rect = form.ClientRectangle
        try
            let backgroundImage = Image.FromFile("E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/cinema.jpg")
            g.DrawImage(backgroundImage, rect)
        with
        | ex -> 
            MessageBox.Show(sprintf "Error loading background image: %s" ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

// Seat Booking Page
let  showMovieSeats (username: string ,movieId: int,movieName:string )=
    let form = new Form(Text = $"Seats for Movie: {movieName}", Size = Size(1000, 600))
    createBackground form
    let  seats = DisplaySeats movieId
    let seatWidth, seatHeight = 80, 60
    let horizontalSpacing, verticalSpacing = 20, 20
    let groupSpacing = 40 

   
    let seatsPerRow = 8
    let totalWidth = seatsPerRow * (seatWidth + horizontalSpacing) - horizontalSpacing + groupSpacing
    let startX = (form.ClientSize.Width - totalWidth) / 2
    let startY = 100 

    let seatButtons =
        seats
        |> List.mapi (fun i (row, col, available) ->
            let offset = if (i % seatsPerRow) >= seatsPerRow / 2 then groupSpacing else 0
            let seatButton = new Button(
                Text = $"{row},{col}",
                Size = Size(seatWidth, seatHeight),
                Location = Point(
                    startX + (i % seatsPerRow) * (seatWidth + horizontalSpacing) + offset,
                    startY + (i / seatsPerRow) * (seatHeight + verticalSpacing)
                ),
                BackColor = if available then Color.Green else Color.Red
            )
            let mutable AV =available
            seatButton.Click.AddHandler(EventHandler(fun _ _ ->
                if AV && seatButton.BackColor =Color.Green then
                    let result = bookSeat username movieId row col
                    MessageBox.Show(result) |> ignore
                    AV <- false
                    seatButton.BackColor <- Color.Red
                else
                    let mutable checkTicket = checkTicket movieId username row col
                    if checkTicket && seatButton.BackColor =Color.Red then
                        //let cancelForm = new Form(Text = "Cancel Booking?", Size = Size(350, 200))
                        //let cancelBtnOk = new Button(Text = "OK", Location = Point(200, 100))
                        //let cancelBtnNo = new Button(Text = "Cancel", Location = Point(50, 100))
                        //cancelForm.Controls.Add(cancelBtnOk)
                        //cancelForm.Controls.Add(cancelBtnNo)
                        //cancelForm.Show()
                        let message =MessageBox.Show("Do you want to cancel your reservation?", "Cancel Reservation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        if message= DialogResult.Yes then
                            MessageBox.Show(bookCancle movieId username row col) |> ignore
                            seatButton.BackColor <- Color.Green
                            AV <- true

                        //cancelBtnNo.Click.AddHandler(EventHandler(fun _ _ -> cancelForm.Hide()))
                        //cancelBtnOk.Click.AddHandler(EventHandler(fun _ _ ->
                        //    MessageBox.Show(bookCancle movieId username row col) |> ignore
                        //    cancelForm.Hide()
                        //    seatButton.BackColor <- Color.Green
                        //))
                    else
                        MessageBox.Show("This seat is reserved by someone else.") |> ignore
            ))
            form.Controls.Add(seatButton)
        )

    let screenLabel = new Label(
        Text = "Seats",
        Location = Point((form.ClientSize.Width - 500) / 2, 50),
        Size = Size(500, 40),
        Font = new Font("Arial", 18.0f, FontStyle.Bold),
        ForeColor = Color.Black,
        TextAlign = ContentAlignment.MiddleCenter,
        BackColor = Color.LightGray
    )
    form.Controls.Add(screenLabel)

    let backButton = new Button(
        Text = "Back to Movies",
        Location = Point(10, 10),
        Size = Size(150, 40),
        BackColor = Color.DarkRed,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
    )
    backButton.Click.Add(fun _ ->
        form.Hide() 
        (mainPage username).Show()
    )
    form.FormClosed.Add(fun _ -> Application.Exit())
    form.Controls.Add(backButton)
    form



// Main Page ( Movies page)
let mainPage (username:string):Form =
    let form = new Form(Text = "Movies", Size = Size(950, 600))
    form.StartPosition <- FormStartPosition.CenterScreen 

    let movies = displayMovies

    let moviesSticker = [
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/bank-elhaz.jpg";
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/el3almy.jpg";
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/el3amel zero.jpg";
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/el7arefa.jpg";
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/kera w elgen.jpg";
        "E:/mostafa/level 4/PL3/project/Cinema Seat Reservation System/Pl3_Project/pictures/welad rezk.jpg"
    ]
    let flowLayoutPanel = new FlowLayoutPanel(Dock = DockStyle.Fill, AutoScroll = true)
    form.Text <- "Select a Movie"
    form.BackColor <- Color.FromArgb(40, 0, 0) 
    form.ForeColor <- Color.White 


    let backButton = new Button(
        Text = "Log Out",
        Location = Point(10, 10),
        Size = Size(150, 40),
        BackColor = Color.DarkRed,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
    )
    backButton.Click.Add(fun _ ->
        form.Hide() // Hide the current form
        loginPage().Show()
    )
    form.Controls.Add(backButton)

    movies
    |> List.mapi (fun i (id,name, time) ->
                  let panel = new Panel(Size = Size(200, 320), Margin = Padding(10), BackColor = Color.Transparent)
                  let pictureBox = new PictureBox(Size = Size(200, 200), SizeMode = PictureBoxSizeMode.StretchImage, ImageLocation = moviesSticker[i])
                  let label = new Label(Text = name, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Font = new Font("Arial", 12.0f, FontStyle.Bold), ForeColor = Color.White)
                  pictureBox.Location <- Point(0,50)
                  let movieButton = new Button(Text = "Select", Size = Size(180, 40), Location = Point(10, 255), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
                  movieButton.Font <- new Font("Arial", 10.0f, FontStyle.Bold)
                  movieButton.FlatAppearance.BorderSize <- 1
                  movieButton.FlatAppearance.BorderColor <- Color.White
                  movieButton.Click.AddHandler(EventHandler(fun _ _ ->
                                                       (form.Hide()
                                                        showMovieSeats(username,id,name)).Show() |> ignore
        
                     ))
                  panel.Controls.Add(pictureBox)
                  panel.Controls.Add(label)
                  panel.Controls.Add(movieButton)
                  flowLayoutPanel.Controls.Add(panel)
                  form.Controls.Add(flowLayoutPanel) ) |> ignore
    createBackground(form)
    
    form.FormClosed.Add(fun _ -> Application.Exit())
    form






///////zanon///////////////////

/////////////mostafa waleed/////////////////////////



// Signup Page
let signupPage () =
    let form = new Form(Text = "Sign Up", Size = Size(800, 600))
    form.StartPosition <- FormStartPosition.CenterScreen 

    createBackground(form)

    let headerLabel = new Label(
        Text = "Welcome to Cinema - Sign Up",
        Location = Point((form.ClientSize.Width - 500) / 2, 50),   
        AutoSize = true,
        Font = new Font("Arial", 30.0f, FontStyle.Bold),
        ForeColor = Color.White,
        BackColor = Color.Transparent
    )
    let lblUsername = new Label(Text = "Username:", Location = Point(200, 150), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtUsername = new TextBox(Location = Point(400, 150), Width = 250, Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)

    let lblPassword = new Label(Text = "Password:", Location = Point(200, 220), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtPassword = new TextBox(Location = Point(400, 220), Width = 250, PasswordChar = '*', Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)
    txtPassword.PasswordChar <- '*'

    let lblName = new Label(Text = "Full Name:", Location = Point(200, 290), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtName = new TextBox(Location = Point(400, 290), Width = 250, Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)

    let lblPhone = new Label(Text = "Phone Number:", Location = Point(200, 360), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtPhone = new TextBox(Location = Point(400, 360), Width = 250, Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)

    let btnSignup = new Button(Text = "Sign Up", Location = Point(400, 430), Width = 150, Height = 50, BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat)

    

    btnSignup.Click.AddHandler(EventHandler(fun _ _ ->
        let result = signup txtName.Text (getIntValue(txtPhone.Text)) txtUsername.Text txtPassword.Text
        if result = "User added successfully!" then
            MessageBox.Show("success!") |>ignore
            form.Hide()
        else
            MessageBox.Show(result) |>ignore
    ))
    form.Controls.Add(lblName)
    form.Controls.Add(txtName)
    form.Controls.Add(lblPhone)
    form.Controls.Add(txtPhone)
    form.Controls.Add(lblUsername)
    form.Controls.Add(txtUsername)
    form.Controls.Add(lblPassword)
    form.Controls.Add(txtPassword)
    form.Controls.Add(btnSignup)
    form.Controls.Add(headerLabel)
    form.FormClosed.Add(fun _ -> Application.Exit())
    form






//////////////mostafa waleed///////////////////////////






///////////abdalla//////////////////////////


// Login Page
let loginPage _ :Form =

    let form = new Form(Text = "Login", Size = Size(800, 600)) 
    form.StartPosition <- FormStartPosition.CenterScreen 

    createBackground(form)

    let headerLabel = new Label(
        Text = "Welcome to Cinema",
        Location = Point((form.ClientSize.Width - 500) / 2, 50), 
        AutoSize = true,
        Font = new Font("Arial", 30.0f, FontStyle.Bold),
        ForeColor = Color.White,
        BackColor = Color.Transparent
    )
    let lblUsername = new Label(Text = "Username:", Location = Point(200, 150), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtUsername = new TextBox(Location = Point(400, 150), Width = 250, Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)

    let lblPassword = new Label(Text = "Password:", Location = Point(200, 220), AutoSize = true, Font = new Font("Arial", 18.0f), BackColor = Color.Transparent, ForeColor = Color.White)
    let txtPassword = new TextBox(Location = Point(400, 220), Width = 250, PasswordChar = '*', Font = new Font("Arial", 16.0f), ForeColor = Color.White, BackColor = Color.Black)
    txtPassword.PasswordChar <- '*'

    let btnLogin = new Button(Text = "Login", Location = Point(400, 300), Width = 150, Height = 50, BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat)

    let btnSignup = new Button(Text = "Sign Up", Location = Point(560, 300), Width = 150, Height = 50, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
    
    btnLogin.Click.AddHandler(EventHandler(fun _ _ ->
        let result =   login txtUsername.Text txtPassword.Text
        match result with
                | "" -> 
                        MessageBox.Show("Invalid login") |> ignore   
                | str -> 
                       form.Hide()
                       
                       mainPage(str).Show()
    ))
    
    btnSignup.Click.AddHandler(EventHandler(fun _ _ ->
        
       // signupPage().Show()

    ))
    form.Controls.Add(lblUsername)
    form.Controls.Add(txtUsername)
    form.Controls.Add(lblPassword)
    form.Controls.Add(txtPassword)
    form.Controls.Add(btnLogin)
    form.Controls.Add(btnSignup)
    form.Controls.Add(headerLabel)
    //return the form

    form.FormClosed.Add(fun _ -> Application.Exit())

    form
    
[<EntryPoint>]
let main argv =

    Application.Run(loginPage ())
    0 


    //////abdalla//////
