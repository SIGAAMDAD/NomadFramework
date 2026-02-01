$newCopyright = @"
/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/
"@

$agplCopyright = @"
/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/
"@

$oldMplCopyright = @"
/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/
"@

$sourceDir = "c:\Users\nyvan\OneDrive\Documents\GitHub\NomadFramework\Source"

$agplFiles = Get-ChildItem -Path $sourceDir -Recurse -Filter *.cs | Where-Object { (Get-Content $_.FullName -Raw) -match [regex]::Escape($agplCopyright) }

$oldMplFiles = Get-ChildItem -Path $sourceDir -Recurse -Filter *.cs | Where-Object { (Get-Content $_.FullName -Raw) -match [regex]::Escape($oldMplCopyright) }

$replaced = 0

foreach ($file in $agplFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $newContent = $content -replace [regex]::Escape($agplCopyright), $newCopyright
    Set-Content -Path $file.FullName -Value $newContent
    $replaced++
    Write-Host "Replaced AGPL in $($file.FullName)"
}

foreach ($file in $oldMplFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $newContent = $content -replace [regex]::Escape($oldMplCopyright), $newCopyright
    Set-Content -Path $file.FullName -Value $newContent
    $replaced++
    Write-Host "Replaced old MPL in $($file.FullName)"
}

Write-Host "Total replaced: $replaced"