import { Component } from 'react'

import { completeSignIn } from '../authService'

export class AuthCallback extends Component {
	async componentDidMount() {
		await completeSignIn()
		this.props.history.push('/')
	}

	render() {
		return null
	}
}