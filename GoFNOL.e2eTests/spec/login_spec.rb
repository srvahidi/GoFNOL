require 'spec_helper'

describe 'login spec', :type => :feature do
	it 'should be able to reach login page' do
		visit '/account'

		expect(page).to have_content 'Pass:'
	end
end