$Const = [PSCustomObject]@{
    Command = @{
        Help = "/help"
        Clear = "/cls"
        Reset = "/rs"
        Exit = "/end"
    }
    Message = [PSCustomObject]@{
        Funny = @(
            "???", 
            "(Silence...) Try typing something!", 
            "Hello...? Anyone there? 👀", 
            "Your keyboard okay? 🧐", 
            "Ah, the sound of nothingness... poetic!", 
            "Is this a game of charades? 😆", 
            "Type something, I promise I won't bite! 🐉"
        )
        Error = "Error"
        Start = "Hello, let's talk! Type [{0}] to see available options..."
        Menu = "Available commands: `n[{0}] - Show this menu` `n[{1}] - Clear chat history` `n[{2}] - Clear screen` `n[{3}] - Exit"
        Clear = "Screen has been cleared!"
        Reset = "Chat history has been reset!"
        Exit = "Goodbye! Have a great day! 👋"
    }
    Dump = @{
        Alice = "Alice"
        Bob   = "Bob"
    }
    Config = @{
        BaseUrl = "https://openrouter.ai/api/v1/chat/completions"
        Schema = "Bearer"
        ApiKey = "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"
        Model = "deepseek/deepseek-chat-v3-0324:free"    
    }
    Role = @{
        User = "user"
        Assistant = "assistant"
    }
}
$Request = [PSCustomObject]@{
    Header = @{
        Key = @{
            ContentType = "Content-Type"
            Auth = "Authorization"
        }
        Value = @{
            ContentType = "application/json"
            Auth = "$($Const.Config.Schema) $($Const.Config.ApiKey)"
        }
    }
    Body = @{
        model = $Const.Config.Model
        messages = @()
    }
}
function input {
    param (
        [System.ConsoleColor]$color = [System.ConsoleColor]::Magenta
    )

    Write-Host "$($Const.Dump.Bob): " -ForegroundColor $color -NoNewline
    return Read-Host
}
function output {
    param (
        $content = "$($Const.Dump.Alice): ",
        [System.ConsoleColor]$color = [System.ConsoleColor]::Magenta,
        [bool]$noNewline = $true
    )

    if ($noNewline) {
        Write-Host $content -ForegroundColor $color -NoNewline
    } else {
        Write-Host $content -ForegroundColor $color
    }
}
function Add-Message {
    param (
        [string]$role = $Const.Role.User,
        [string]$type = "text",
        [string]$text
    )
    
    $message = [PSCustomObject]@{
        role = $role
        content = @(@{
            type = $type
            text = $text
        })
    }

    $Request.Body.messages += $message
}
function Send-Request {
    param (
        [string]$prompt
    )

    Add-Message -text $prompt
    $body = $Request.Body | ConvertTo-Json -Depth 10

    try {
        [System.Console]::CursorVisible = $false
        output
        output "..."

        $sending = Start-Job -ScriptBlock {
            param ($url, $header, $body)

            return Invoke-RestMethod -Uri $url -Method Post -Headers $header -Body $body
        } -ArgumentList $Const.Config.BaseUrl, @{
            $Request.Header.Key.Auth = $Request.Header.Value.Auth
            $Request.Header.Key.ContentType = $Request.Header.Value.ContentType
        }, $body

        Wait-Job -Job $sending | out-null
        $response = Receive-Job -Job $sending
        Remove-Job -Job $sending

        if ($response) {
            $message = $response.choices[0].message.content
            [System.Console]::SetCursorPosition([System.Console]::CursorLeft - 3, [System.Console]::CursorTop)
            output $message Green $false
            Add-Message -role $Const.Role.Assistant -text $message  
        }
    }
    catch {
        output "$($Const.Message.Error): $_." Red $false
    }
}
function main {
    output ($Const.Message.Start -f $Const.Command.Help) Cyan $false

    :exitinput while ($true) {
        [System.Console]::CursorVisible = $true # de o day de cursor luon visible khi input
        $isSkip = $false # ko dung continue duoc
        $inputUser = input

        switch ($inputUser.ToLower()) {
            $Const.Command.Help {
                output
                output ($Const.Message.Menu -f $Const.Command.Help, $Const.Command.Clear, $Const.Command.Reset, $Const.Command.Exit) Cyan $false
                $isSkip = $true
            }
            $Const.Command.Clear {
                Clear-Host
                output
                output $Const.Message.Clear Cyan $false
                $isSkip = $true
            }
            $Const.Command.Reset {
                output
                output $Const.Message.Reset Cyan $false
                $Request.Body.messages.Clear()
                $isSkip = $true
            }
            $Const.Command.Exit {
                output
                output $Const.Message.Exit Cyan $false
                break exitinput
            }
            default {
                if ([string]::IsNullOrWhiteSpace($inputUser)) {
                    output
                    output ($Const.Message.Funny | Get-Random) Yellow $false
                    $isSkip = $true
                }
            }
        }
        if ($isSkip -eq $false) {
            Send-Request $inputUser
        }
    }
}
main