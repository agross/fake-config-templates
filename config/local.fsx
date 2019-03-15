#load "./default.fsx"

open Configuration.Default

// Uncomment if you want to change defaults
Config <- { Config with
                 Bar = 23
                 BazIsOptional = Some(42) }
