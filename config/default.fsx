namespace Configuration

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

module Default =

  let mutable Config =
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
