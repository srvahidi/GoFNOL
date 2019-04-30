import { Component } from 'react'

export class AuthCallback extends Component {
	async componentDidMount() {
		await this.props.authService.completeSignIn()
		this.props.history.push('/')
	}

	render() {
		return null
	}
}