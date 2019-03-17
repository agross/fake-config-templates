open System

type DatabaseConfig =
  {
    ConnectionString: string
    User: string
    PortIsOptional: int option
  }

type Config =
  {
    Foo: string
    Bar: int
    BazIsOptional: int option
    Db: DatabaseConfig
  }

let DefaultConfig =
  {
    Foo = "foo is foo"
    Bar = 42
    BazIsOptional = None
    Db = {
      ConnectionString = "blah"
      User = "sa"
      PortIsOptional = None
    }
  }

let Config =
  match Environment.UserName with
  | "agross" -> {  DefaultConfig with
                                Bar = 23
                                BazIsOptional = Some(42)}
  | _ -> DefaultConfig
