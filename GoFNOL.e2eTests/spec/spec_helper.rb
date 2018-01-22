require 'capybara/rspec'
require 'httparty'


if ENV["FNOL_STARTUP_ATTEMPTS"].to_i > 0
  FNOL_STARTUP_ATTEMPTS = ENV["FNOL_STARTUP_ATTEMPTS"].to_i
else
  FNOL_STARTUP_ATTEMPTS = 30
end

Capybara.register_driver :chrome do |app|
  Capybara::Selenium::Driver.new(app, :browser => :chrome)
end

#Capybara.run_server = false
Capybara.default_driver = :chrome
Capybara.javascript_driver = :chrome
SERVER_URL = ENV["SERVER_URL"] || "http://127.0.0.1:5000"
Capybara.app_host = SERVER_URL

RSpec.configure do |config|
  pid = nil
  log_lines = nil
  server_started = false
  any_test_failed = false

  config.before(:suite) do
    if server_down?(SERVER_URL)
      pid, log_lines = start_fnol(SERVER_URL)
    end
    server_started = wait_for_server(SERVER_URL)
	
    if !server_started then
      any_test_failed = true
      raise "Application failed to start!"
    end
  end

  config.after(:suite) do
    if any_test_failed then
      puts "Failed test. Server logs follow:"
	  # TODO restore
      #until log_lines.empty?
        #puts log_lines.deq
      #end
    end
	
    if server_started then
      stop_fnol(pid)
    end
  end

  config.around(:each) do |example|
    example.run

    any_test_failed ||= example.exception
  end
end

def server_down?(server_url)
  begin
    code = HTTParty.get("#{server_url}/Account").response.code.to_i
    code < 200 || code > 299
  rescue
    true
  end
end

def start_fnol(server_url)
  puts "Starting FNOL with url #{server_url}..."
  pid = nil
  output_lines = Queue.new
  ENV["ASPNETCORE_URLS"] = server_url

  Dir.chdir('../GoFNOL') do
    r, w = IO.pipe
    pid = Process.spawn("dotnet run", :out => w)
    Thread.new do
      r.each_line.each do |line|
        output_lines.enq line
      end
      w.close
    end
  end
  return pid, output_lines
end

def wait_for_server(server_url)
  num_tries = FNOL_STARTUP_ATTEMPTS
  started = false
  while (num_tries > 0 && !started) do
    puts "Attempting #{num_tries}/#{FNOL_STARTUP_ATTEMPTS} more times to reach #{server_url}/Account"
    begin
      code = HTTParty.get("#{server_url}/Account").response.code.to_i

      puts "Received code: #{code}"
      started = 200 <= code && code < 300
    rescue
    end

    num_tries -= 1

    sleep 1 unless started
  end
  started
end

def stop_fnol(pid)
  return unless pid
  if Gem.win_platform?
    system("taskkill /f /t /pid #{pid}")
  else
    # Get the child pids.
    pipe = IO.popen("ps -ef | grep #{pid}")

    pipe.readlines.map do |line|
      parts = line.split(/\s+/)
      parts[2] if parts[3] == pid.to_s and parts[2] != pipe.pid.to_s
    end.compact.each {|cpid|
      # Show the child processes.
      puts "\nShutting down process: #{cpid}"
      Process.kill('KILL', cpid.to_i)
    }
  end
end