param(
    [string] $Data,

    [string] $DataPath,

    [string] $Method,

    [string] $Proto = "../../proto/Narrator/Narrator.proto",

    [string] $ServiceId = "Atlas.Narrator.Narrator",
    
    [string] $Url = "localhost:50051"
)

if ($PSBoundParameters.ContainsKey('Data')) {
    $dataToSend = $Data
}
else {
    $dataToSend = Get-Content $DataPath -Raw
}

$ServiceId = "Atlas.Narrator.Narrator"

grpcurl -proto $Proto -import-path "../../proto" -plaintext -d "$dataToSend" $Url "$ServiceId/$Method"