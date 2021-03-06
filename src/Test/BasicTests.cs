using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Sheller.Implementations;
using Sheller.Implementations.Executables;
using Sheller.Implementations.Shells;
using Xunit;

// NOTE: win tests require WSL...because...lazy.

namespace Sheller.Tests
{
    public class BasicTests
    {
        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoBash()
        {
            var expected = "lol";

            var echoValue = await Builder
                .UseShell("bash")
                .UseExecutable("echo")
                    .WithArgument(expected)
                .ExecuteAsync();

            Assert.True(echoValue.Succeeded);
            Assert.Equal(0, echoValue.ExitCode);
            Assert.Equal(expected, echoValue.StandardOutput.Trim());
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithGeneric()
        {
            var expected = "lol";

            var echoValue = await Builder
                .UseShell<Bash>()
                .UseExecutable<Echo>()
                    .WithArgument(expected)
                .ExecuteAsync();

            Assert.Equal(expected, echoValue);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithSwappedShell()
        {
            var expected = "lol";
            
            var echoValue = await Builder.UseShell("not_a_shell_lol").UseExecutable<Echo>()
                .UseShell(Builder.UseShell<Bash>())
                .WithArgument(expected)
                .ExecuteAsync();

            Assert.Equal(expected, echoValue);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanThrowWithBadExe()
        {
            var expected = 127;

            var exception = await Assert.ThrowsAsync<ExecutionFailedException>(async () =>
            {
                var echoValue = await Builder
                .UseShell<Bash>()
                .UseExecutable("foo")
                .ExecuteAsync();
            });

            Assert.False(exception.Result.Succeeded);
            Assert.NotEqual(0, exception.Result.ExitCode);
            Assert.Equal(expected, exception.Result.ExitCode);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanSuppressThrowWithBadExe()
        {
            var expected = 127;

            var echoValue = await Builder
                .UseShell<Bash>()
                .UseExecutable("foo")
                .UseNoThrow()
                .ExecuteAsync();

            Assert.Equal(expected, echoValue.ExitCode);
        }

        [Fact]
        [Trait("os", "nix")]
        public async void CanExecuteEchoWithGenericAndEnvironmentVariableNix()
        {
            var expected = "lol";

            var echoValue = await Builder
                .UseShell<Bash>()
                    .WithEnvironmentVariable("MY_VAR", expected)
                .UseExecutable<Echo>()
                    .WithArgument("$MY_VAR")
                .ExecuteAsync();

            Assert.Equal(expected, echoValue);
        }

        [Fact]
        [Trait("os", "win")]
        public async void CanExecuteEchoWithGenericAndEnvironmentVariablewin()
        {
            var expected = "lol";

            var echoValue = await Builder
                .UseShell<Bash>()
                    .WithEnvironmentVariable("MY_VAR", expected)
                .UseExecutable<Echo>()
                    .WithArgument("\\$MY_VAR")
                .ExecuteAsync();

            Assert.Equal(expected, echoValue);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithTimeout()
        {
            var min = 2;
            var max = 4;

            var start = DateTime.Now;
            await Assert.ThrowsAsync<ExecutionTimeoutException>(() =>
            {
                return Builder
                    .UseShell<Bash>()
                    .UseExecutable<Sleep>()
                        .WithArgument(max.ToString())
                        .UseTimeout(TimeSpan.FromSeconds(min + .1))
                    .ExecuteAsync();
            });
            var delta = DateTime.Now - start;

            Assert.True(delta.TotalSeconds > min);
            Assert.True(delta.TotalSeconds < max);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithStandardOutputHandlerOnShell()
        {
            var expected = "lol";
            var handlerString = new StringBuilder();

            var echoValue = await Builder
                .UseShell<Bash>()
                    .WithStandardOutputHandler(s => handlerString.Append(s))
                .UseExecutable<Echo>()
                    .WithArgument(expected)
                .ExecuteAsync();
                
            Assert.Equal(expected, echoValue);
            Assert.Equal(expected, handlerString.ToString());
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithStandardOutputHandlerOnExecutable()
        {
            var expected = "lol";
            var handlerString = new StringBuilder();

            var echoValue = await Builder
                .UseShell<Bash>()
                .UseExecutable<Echo>()
                    .WithArgument(expected)
                    .WithStandardOutputHandler(s => handlerString.Append(s))
                .ExecuteAsync();
                
            Assert.Equal(expected, echoValue);
            Assert.Equal(expected, handlerString.ToString());
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithStandardErrorHandler()
        {
            var expected = "error";
            var handlerString = new StringBuilder();

            var echoResult = await Builder
                .UseShell<Bash>()
                .UseExecutable(">&2 echo")
                    .WithArgument(expected)
                    .WithStandardErrorHandler(s => handlerString.Append(s))
                .ExecuteAsync();
            
            Assert.Equal(expected, echoResult.StandardError.Trim());
            Assert.Equal(expected, handlerString.ToString());
        }

        [Fact]
        [Trait("os", "nix")]
        public async void CanExecuteEchoWithStandardInputNix()
        {
            var expected1 = "lol";
            var expected2 = "face";

            var echoResult = await Builder
                .UseShell<Bash>()
                .UseExecutable("read var1; read var2; echo $var1$var2")
                    .WithStandardInput(expected1)
                    .WithStandardInput(expected2)
                .ExecuteAsync();
            
            Assert.Equal($"{expected1}{expected2}", echoResult.StandardOutput.Trim());
        }

        [Fact]
        [Trait("os", "win")]
        public async void CanExecuteEchoWithStandardInputWin()
        {
            var expected1 = "lol";
            var expected2 = "face";

            var echoResult = await Builder
                .UseShell<Bash>()
                .UseExecutable("read var1; read var2; echo \\$var1\\$var2")
                    .WithStandardInput(expected1)
                    .WithStandardInput(expected2)
                .ExecuteAsync();
            
            Assert.Equal($"{expected1}\r\n{expected2}", echoResult.StandardOutput.Trim());
        }

        [Fact]
        [Trait("os", "nix")]
        public async void CanExecuteEchoWithInputRequestHandlerNix()
        {
            var expected1 = "hello";
            var expected2 = "lol";

            var echoResult = await Builder
                .UseShell<Bash>()
                .UseExecutable($"echo {expected1}; read var1; echo $var1")
                .UseInputRequestHandler((stdout, stderr) =>
                {
                    Assert.Contains(expected1, stdout);
                    return Task.FromResult(expected2);
                })
                .ExecuteAsync();
            
            Assert.Contains($"{expected2}", echoResult.StandardOutput);
        }

        [Fact]
        [Trait("os", "win")]
        public async void CanExecuteEchoWithInputRequestHandlerWin()
        {
            var expected1 = "hello";
            var expected2 = "lol";

            var echoResult = await Builder
                .UseShell<Bash>()
                .UseExecutable($"echo {expected1}; read var1; echo \\$var1")
                .UseInputRequestHandler((stdout, stderr) =>
                {
                    Assert.Contains(expected1, stdout);
                    return Task.FromResult(expected2);
                })
                .ExecuteAsync();
            
            Assert.Contains($"{expected2}", echoResult.StandardOutput);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithResultSelector()
        {
            var expected = 0;

            var echoErrorCode = await Builder
                .UseShell<Bash>()
                .UseExecutable<Echo>()
                    .WithArgument("dummy")
                .ExecuteAsync(cr => 
                {
                    return cr.ExitCode;
                });
                
            Assert.Equal(expected, echoErrorCode);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithResultSelectorTask()
        {
            var expected = 0;

            var echoErrorCode = await Builder
                .UseShell<Bash>()
                .UseExecutable<Echo>()
                    .WithArgument("dummy")
                .ExecuteAsync(async cr => 
                {
                    await Task.Delay(100);
                    return cr.ExitCode;
                });
                
            Assert.Equal(expected, echoErrorCode);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithWait()
        {
            var min = 2;

            var start = DateTime.Now;
            var echoValue = await Builder
                .UseShell<Bash>()
                .UseExecutable<Echo>()
                    .WithArgument("dummy")
                    .WithWait(async cr => await Task.Delay(TimeSpan.FromSeconds(min - 1)))
                    .WithWait(async cr => await Task.Delay(TimeSpan.FromSeconds(min)))
                .ExecuteAsync();
            var delta = DateTime.Now - start;

            Assert.True(delta.TotalSeconds > min);
        }

        [Fact]
        [Trait("os", "nix_win")]
        public async void CanExecuteEchoWithWaitTimeout()
        {
            var min = 2;
            var max = 4;

            var start = DateTime.Now;
            await Assert.ThrowsAsync<ExecutionTimeoutException>(() =>
            {
                return Builder
                    .UseShell<Bash>()
                    .UseExecutable<Echo>()
                        .WithArgument("dummy")
                        .WithWait(async cr => await Task.Delay(TimeSpan.FromSeconds(max)))
                        .WithWait(async cr => await Task.Delay(TimeSpan.FromSeconds(max + 1)))
                        .UseWaitTimeout(TimeSpan.FromSeconds(min))
                    .ExecuteAsync();
            });
            var delta = DateTime.Now - start;

            Assert.True(delta.TotalSeconds > min);
            Assert.True(delta.TotalSeconds < max);
        }
    }
}
