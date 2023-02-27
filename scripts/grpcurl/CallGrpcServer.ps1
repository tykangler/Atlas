param(
    [string] $Proto = "../../proto/coreference_resolver.proto",
    
    [string] $Data,

    [string] $DataPath,

    [string] $Method,

    [string] $Url = "localhost:50051"
)

if ($PSBoundParameters.ContainsKey('Data')) {
    $dataToSend = $Data
} else {
    $dataToSend = Get-Content $DataPath -Raw
}

$ServiceId = "Atlas.CoreferenceResolver.CoreferenceResolver"

grpcurl -proto "$Proto" -plaintext -d "$dataToSend" $Url "$ServiceId/$Method"